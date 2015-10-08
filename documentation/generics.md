<!--Title:Generic Types-->

StructureMap comes with some power abilities to exploit [open generic types](https://msdn.microsoft.com/en-us/library/ms172334(v=vs.110).aspx) in .Net for extensibility
and flexible handling within your system.

## Example 1: Visualizing an Activity Log

I worked years ago on a system that could be used to record and resolve customer support problems. Since it was very workflow heavy in its logic,
we tracked user and system activity as an _event stream_ of small objects that reflected all the different actions or state changes
that could happen to an issue. To render and visualize the activity log to HTML, we used many of the open generic type capabilities shown in
this topic to find and apply the correct HTML rendering strategy for each type of log object in an activity stream.

Given a log object, we wanted to look up the right visualizer strategy to render that type of log object to html on the server side.

To start, we had an interface like this one that we were going to use to get the HTML for each log object:

<[sample:ILogVisualizer]>

So for an example, if we already knew that we had an `IssueCreated` object, we should be able to use StructureMap like this:

<[sample:using-visualizer-knowning-the-type]>

If we had an array of log objects, but we do not already know the specific types, we can still use the more generic `ToHtml(object)` method like this:

<[sample:using-visualizer-not-knowing-the-type]>

The next step is to create a way to identify the visualization strategy for a single type of log object. We certainly could have done this
with a giant switch statement, but we wanted some extensibility for new types of activity log objects and even customer specific log types
that would never, ever be in the main codebase. We settled on an interface like the one shown below that would be responsible for
rendering a particular type of log object ("T" in the type):


<[sample:IVisualizer<T>]>

Inside of the concrete implementation of `ILogVisualizer` we need to be able to pull out and use the correct `IVisualizer<T>` strategy for a log type. We of course
used a StructureMap `Container` to do the resolution and lookup, so now we also need to be able to register all the log visualization strategies in some easy way.
On top of that, many of the log types were simple and could just as easily be rendered with a simple html strategy like this class:

<[sample:DefaultVisualizer]>

Inside of our StructureMap usage, if we don't have a specific visualizer for a given log type, we'd just like to fallback to the default visualizer and proceed.

Alright, now that we have a real world problem, let's proceed to the mechanics of the solution.




## Registering Open Generic Types

Let's say to begin with all we want to do is to always use the `DefaultVisualizer` for each log type. We can do that with code like this below:

<[sample:register_open_generic_type]>



With the configuration above, there are no specific registrations for `IVisualizer<IssueCreated>`. At the first request for that
interface, StructureMap will run through its "<[linkto:registration/on-missing-family-policies;title=missing family policies]>", one of which is
to try to find registrations for an open generic type that could be closed to make a valid registration for the requested type. In the case above,
StructureMap sees that it has registrations for the open generic type `IVisualizer<T>` that could be used to create registrations for the
closed type `IVisualizer<IssueCreated>`. 

Using the <[linkto:diagnostics/whatdoihave]> diagnostics, the original state of the container for the visualization namespace is:

<pre style="overflow:scroll;word-break:normal;word-wrap:normal">
===========================================================================================================================
PluginType            Namespace                                         Lifecycle     Description                 Name     
---------------------------------------------------------------------------------------------------------------------------
IVisualizer&lt;TLog&gt;     StructureMap.Testing.Acceptance.Visualization     Transient     DefaultVisualizer&lt;TLog&gt;     (Default)
===========================================================================================================================
</pre>

After making a request for `IVisualizer<IssueCreated>`, the new state is:

<pre style="overflow:scroll;word-break:normal;word-wrap:normal">
====================================================================================================================================================================================
PluginType                    Namespace                                         Lifecycle     Description                                                                  Name     
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IVisualizer&lt;IssueCreated&gt;     StructureMap.Testing.Acceptance.Visualization     Transient     DefaultVisualizer&lt;IssueCreated&gt; ('548b4256-a7aa-46a3-8072-bd8ef0c5c430')     (Default)
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IVisualizer&lt;TLog&gt;             StructureMap.Testing.Acceptance.Visualization     Transient     DefaultVisualizer&lt;TLog&gt;                                                      (Default)
====================================================================================================================================================================================


</pre>


## Generic Registrations and Default Fallbacks

A powerful feature of generic type support in StructureMap is the ability to register specific handlers for some types, but allow
users to register a "fallback" registration otherwise. In the case of the visualization, some types of log objects may justify some
special HTML rendering while others can happily be rendered with the default visualization strategy. This behavior is demonstrated by
the following code sample:

<[sample:generic-defaults-with-fallback]>


## Connecting Generic Implementations with Type Scanning

<div class="alert alert-info" role="alert">It's generally harmful in software projects to have a single code file that has to be frequently edited to for unrelated changes,
 and StructureMap <code>Registry</code> classes that explicitly configure services can easily fall into that category. Using type scanning registration can help
 teams avoid that problem altogether by eliminating the need to make any explict registrations as new providers are added to the codebase.</div>



For this example, I have two special visualizers for the `IssueCreated` and `IssueResolved` log types:

<[sample:specific-visualizers]>

In the real project that inspired this example, we had many, many more types of log visualizer strategies and it 
could have easily been very tedious to manually register all the different little `IVisualizer<T>` strategy types in a `Registry` class by hand. 
Fortunately, part of StructureMap's <[linkto:registration/auto-registration-and-conventions;title=type scanning]> support is the `ConnectImplementationsToTypesClosing()`
auto-registration mechanism via generic templates for exactly this kind of scenario.

In the sample below, I've set up a type scanning operation that will register any concrete type in the Assembly that contains the `VisualizationRegistry`
that closes `IVisualizer<T>` against the proper interface:


<[sample:VisualizationRegistry]>

If we create a `Container` based on the configuration above, we can see that the type scanning operation picks up the specific visualizers for
`IssueCreated` and `IssueResolved` as shown in the diagnostic view below:


<pre style="overflow:scroll;word-break:normal;word-wrap:normal">
==================================================================================================================================================================================
PluginType                     Namespace                                         Lifecycle     Description                                                               Name     
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
ILogVisualizer                 StructureMap.Testing.Acceptance.Visualization     Transient     StructureMap.Testing.Acceptance.Visualization.LogVisualizer               (Default)
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IVisualizer&lt;IssueResolved&gt;     StructureMap.Testing.Acceptance.Visualization     Transient     StructureMap.Testing.Acceptance.Visualization.IssueResolvedVisualizer     (Default)
                                                                                 Transient     DefaultVisualizer&lt;IssueResolved&gt;                                                   
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IVisualizer&lt;IssueCreated&gt;      StructureMap.Testing.Acceptance.Visualization     Transient     StructureMap.Testing.Acceptance.Visualization.IssueCreatedVisualizer      (Default)
                                                                                 Transient     DefaultVisualizer&lt;IssueCreated&gt;                                                    
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IVisualizer&lt;TLog&gt;              StructureMap.Testing.Acceptance.Visualization     Transient     DefaultVisualizer&lt;TLog&gt;                                                   (Default)
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
IVisualizer&lt;TLog&gt;              StructureMap.Testing.Acceptance.Visualization     Transient     DefaultVisualizer&lt;TLog&gt;                                                   (Default)
==================================================================================================================================================================================

</pre>

The following sample shows the `VisualizationRegistry` in action to combine the type scanning registration plus the default fallback behavior for
log types that do not have any special visualization logic:

<[sample:visualization-registry-in-action]>



## Building Closed Types with ForGenericType() and ForObject()

Working with generic types and the common `IHandler<T>` pattern can be a little bit tricky if all you have is an object that is declared as an `object`. Fortunately,
StructureMap has a couple helper methods and mechanisms to help you bridge the gap between `DoSomething(object something)` and `DoSomething<T>(T something)`.

If you remember the full `ILogVisualizer` interface from above:

<[sample:ILogVisualizer]>

The method `ToHtml(object log)` somehow needs to be able to find the right `IVisualizer<T>` and execute it to get the HTML representation for a log object.
The StructureMap `IContainer` provides two different methods called `ForObject()` and `ForGenericType()` for exactly this case, as shown below in a possible implementation
of `ILogVisualizer`:

<[sample:LogVisualizer]>

The two methods are almost identical in result with some slight differences:

1. `ForObject(object subject)` can only work with open types that have only one generic type parameter, and it will pass the argument `subject` to the underlying `Container` as an <[linkto:resolving/passing-arguments-at-runtime;title=explicit argument]> so that you can inject that `subject` object into the object graph being created.
1. `ForGenericType(Type openType)` is a little clumsier to use, but can handle any number of generic type parameters





## Example #2: Generic Instance Builder

As I recall, the following example was inspired by a question about how to use StructureMap to build out MongoDB [MongoCollection](http://api.mongodb.org/csharp/1.7/html/6d5bbe8e-46f4-087f-8e83-06297634ed40.htm) objects from some sort of static builder or factory -- but I can't find the discussion on the mailing list as I write this today. This has come up often enough to justify its inclusion in the documentation.

Say that you have some sort of persistence tooling that you primarily interact with through an interface like this one below, where `TDocument` and `TQuery` are classes in
your persistent domain:

<[sample:IRepository<T,T1>]> 

Great, StructureMap handles generic types just fine, so you can just register the various closed types and off you go. Except you can't because the way that your
persistence tooling works requires you to create the `IRepository<,>` objects with a static builder class like this one below:

<[sample:RepositoryBuilder]>

StructureMap has an admittedly non-obvious way to handle this situation by creating a new subclass of `Instance` that will "know" how to create the real `Instance` for a closed
type of `IRepository<,>`. 

First off, let's create a new `Instance` type that knows how to build a specific type of `IRepository<,>` by subclassing the `LambdaInstance` type and providing a `Func` to
build our repository type with the static `RepositoryBuilder` class:

<[sample:RepositoryInstance]>

As you've probably surmised, the custom `RepositoryInstance` above is itself an open generic type and cannot be used directly until it has been closed. You **could** use this class directly if you have a very few document types like this:

<[sample:using-repository-instance]>

To handle the problem in a more generic way, we can create a second custom subclass of `Instance` for the open type `IRepository<,>` that will help StructureMap understand how
to build the specific closed types of `IRepository<,>` at runtime:

<[sample:RepositoryInstanceFactory]>

The key part of the class above is the `CloseType(Type[] types)` method. At that point, we can determine the right type of `RepositoryInstance<,>` to build the requested type of `IRepository<,>`, then use some reflection to create and return that custom `Instance`.

Here's a unit test that exercises and demonstrates this functionality from end to end:

<[sample:generic-builders-in-action]>

After requesting `IRepository<string, int>` for the first time, the container configuration from <[linkto:diagnostics/whatdoihave;title=Container.WhatDoIHave()]> is:

<pre style="overflow:scroll;word-break:normal;word-wrap:normal">
===================================================================================================================================================
PluginType                         Namespace                           Lifecycle     Description                                          Name     
---------------------------------------------------------------------------------------------------------------------------------------------------
IRepository&lt;String, Int32&gt;         StructureMap.Testing.Acceptance     Transient     RepositoryBuilder.Build&lt;String, Int32&gt;()             (Default)
---------------------------------------------------------------------------------------------------------------------------------------------------
IRepository&lt;TDocument, TQuery&gt;     StructureMap.Testing.Acceptance     Transient     Build Repository&lt;T, T1&gt;() with RepositoryBuilder     (Default)
===================================================================================================================================================
</pre>



## Example 3: Interception Policy against Generic Types

Several years ago I described an approach for [using an Event Aggregator in a WPF application](http://codebetter.com/jeremymiller/2009/07/24/how-i-m-using-the-event-aggregator-pattern-in-storyteller/) that relied on StructureMap interception to register any object that
StructureMap built with the active [EventAggregator](http://martinfowler.com/eaaDev/EventAggregator.html) for the system *if that object was recognized as a listener to the event aggregator*. I thought that approach worked out quite well, so let's talk about how you could implement
that same design with the improved interception model introduced by StructureMap 3.0 (the event aggregator and StructureMap interception worked out well,
but I'm very happy now that I ditched the old WPF client and replaced it with a web application using React.js instead).

First off, let's say that we're going to have this interface for our event aggregator:

<[sample:IEventAggregator]>

To register a listener for a particular type of event notification, you would implement an interface called `IListener<T>` shown below
and directly add that object to the `IEventAggregator`:

<[sample:IListener<T>]>

In the application I'm describing, all of the listener objects were presenters or screen controls that were created by StructureMap, so it was convenient to allow StructureMap to register newly created objects with the `IEventAggregator` in an activation interceptor.

What we want to do though is have an interception policy that only applies to any concrete type that implements some interface that 
closes `IListener<T>`:

<[sample:EventListenerRegistration]>

To see our new interception policy in action, see [this unit test from GitHub](https://github.com/structuremap/structuremap/blob/master/src/StructureMap.Testing/Samples/Interception/Event_Aggregator_Registration.cs):

<[sample:use_the_event_listener_registration]>

