---
title: Interception Capabilities
layout: default
---

StructureMap 2.5+ added the ability to postprocess or even intercept and replace
the objects being created.  While StructureMap will never include its own Aspect
Oriented Programming model (the world does not need a new one), the interception
techniques shown below could be used to apply runtime AOP from existing AOP
tools like the Policy Injection Application Block from Microsoft.

In general, interception is specified in three ways:

1. `OnCreation()` -- Registers an Action to run against the new object after creation

2. `EnrichWith()` -- Registers a Func that runs against the new object after creation
and gives you the option of returning a different object than the original
object

3. A custom class that implements the TypeInterceptor interface (the runtime model
behind all the interception techniques)

Intercept a Single Instance
---------------------------------

You can specify interception for a single Instance.  This interception could be
combined with other Interception policies, but users should be cautious about
this. 


#### Run an Action Against an Object

Some classes may require some extra bootstrapping work before they are ready to
be used.  While I recommend building classes in such a way that the new objects
are ready to function after calling the constructor function, not every class
you will encounter will follow this rule.  For that reason, StructureMap has the
ability to register an `Action<T>` to run against a newly created object before
it is returned to the requesting code.  You can register that Action on an
individual Instance:

{% highlight csharp %}
public class InterceptionRegistry : Registry
{
    public InterceptionRegistry()
    {
        // Perform an Action<T> upon the object of type T 
        // just created before it is returned to the caller
        ForRequestedType<ClassThatNeedsSomeBootstrapping>().TheDefault.Is
            .OfConcreteType<ClassThatNeedsSomeBootstrapping>()
            .OnCreation(x => x.Start());

        // or...

        // You can also register an Action<IContext, T> to get access
        // to all the services and capabilities of the BuildSession
        ForRequestedType<ClassThatNeedsSomeBootstrapping>().TheDefault.Is
            .OfConcreteType<ClassThatNeedsSomeBootstrapping>()
            .OnCreation((context, x) =>
            {
                var connection = context.GetInstance<IConnectionPoint>();
                x.Connect(connection);
            });

    }
}
{% endhighlight %}


#### Wrap or Substitute the Returned Object

Sometimes you may want to wrap the constructed object in some sort of Decorator
or apply runtime AOP to the new object.  In this case, StructureMap will allow
you to substitute the constructed object for the new wrapped object -- with the
restriction that the object returned must be assignable to the requested
`PluginType`.  Let's consider the case of using a
[Decorator](http://resharper.codebetter.com/blogs/jeremy.miller/archive/2005/09/02/131613.aspx)
pattern to add logging to an existing service:

{% highlight csharp %}
public class LoggingDecorator : IConnectionListener
{
    private readonly IConnectionListener _inner;

    public LoggingDecorator(IConnectionListener inner)
    {
        _inner = inner;
    }
}
{% endhighlight %}

When you register an instance of `IConnectionListener`, you can specify that the
constructed object get wrapped with a decorator using the `EnrichWith()` syntax
like this:

{% highlight csharp %}
public InterceptionRegistry()
{
    ForRequestedType<IConnectionListener>().TheDefault.Is
        .OfConcreteType<ClassThatNeedsSomeBootstrapping>()
        .EnrichWith(x => new LoggingDecorator(x));
}
{% endhighlight %}

In the sample registration above, a call to
Container.`GetInstance<IConnectionListener>()` will result in a new
`ClassThatNeedsSomeBootstrapping` wrapped in a `LoggingDecorator` object:

{% highlight csharp %}
[Test]
public void see_the_enrichment_with_a_decorator_in_action()
{
    var container = new Container(new InterceptionRegistry());
    container.GetInstance<IConnectionListener>()
        .ShouldBeOfType<LoggingDecorator>()
        .Inner.ShouldBeOfType<ClassThatNeedsSomeBootstrapping>();
}
{% endhighlight %}

There is also an overload of `EnrichWith()` that takes in the `IContext` object:

{% highlight csharp %}
ForRequestedType<IConnectionListener>().TheDefault.Is
    .OfConcreteType<ClassThatNeedsSomeBootstrapping>()
    .EnrichWith((context, x) =>
    {
        var connection = context.GetInstance<IConnectionPoint>();
        x.Connect(connection);

        return new LoggingDecorator(x);
    });
{% endhighlight %}

See [Using the Session Context](UsingSessionContext.htm) for more information
on using the `IContext`.

#### With a Custom Interceptor

To write a custom Interceptor for a single Instance, create a new class that
implements the `InstaneInterceptor` interface:

{% highlight csharp %}
public interface InstanceInterceptor
{
    object Process(object target, IContext context);
}
{% endhighlight %}

with a class like this:

{% highlight csharp %}
public class CustomInterceptor : InstanceInterceptor
{
    public object Process(object target, IContext context)
    {
        // manipulate the target object and return a wrapped version
        return wrapTarget(target);
    }

    private object wrapTarget(object target)
    {
        throw new NotImplementedException();
    }
}
{% endhighlight %}

Then, register the a new object instance of the `CustomerInterceptor`:

{% highlight csharp %}
ForRequestedType<IConnectionListener>().TheDefault.Is
    .OfConcreteType<ClassThatNeedsSomeBootstrapping>()
    .InterceptWith(new CustomInterceptor());
{% endhighlight %}


Apply Interception to all Instances of a `PluginType`
---------------------------------

The same `OnCreation()`, `EnrichWith()`, and `InterceptWith()` methods can be
applied to all Instances of a given `PluginType` off of the `ForRequestedType()`
or `BuildInstancesOf()` methods of the [Registry DSL](RegistryDSL.htm):

{% highlight csharp %}
// Place the Interception at the PluginType level
ForRequestedType<IConnectionListener>()
    .OnCreation(x => x.StartConnection())    // OnCreation
    .EnrichWith(x => new LoggingDecorator(x)) // Enrich
    .InterceptWith(new CustomInterceptor())   // Custom Interceptor

    .TheDefaultIsConcreteType<ClassThatNeedsSomeBootstrapping>();
{% endhighlight %}

Note that these methods can be used in combination with each other and even
multiple times for the same type.  All additional calls are additive.  Use with
caution!


Apply Interception to all Types Matching a Criteria
---------------------------------

If an interception policy is simple, you can just register the interception
policy with the `IfTypeMatches( Predicate<Type> ).InterceptWith(   Lambda 
)` syntax:

{% highlight csharp %}
registry.IfTypeMatches(type => type.Equals(typeof (BlueSomething)))
    .InterceptWith(rawInstance => new WrappedSomething((IAnInterfaceOfSomeSort) rawInstance));
{% endhighlight %}

Please note that when StructureMap encounters a new concrete type for the first
time, it searches for all `TypeInterceptors` that match that the concrete type,
and caches these `TypeInterceptors` against the concrete type for future usage. 
The long and short of this is that any filter on the type is only going to be
evaluated once.


Creating a Custom Type Interceptor
---------------------------------

Sooner or later the Fluent Interface registration of `TypeInterceptors` will not
be adequate.  In that case, you can create a custom class that implements the
`TypeInterceptor` interface:

{% highlight csharp %}
/// <summary>
/// A TypeInterceptor that is only applied if the MatchesType()
/// method is true for a given Type
/// </summary>
public interface TypeInterceptor : InstanceInterceptor
{
    /// <summary>
    /// Does this TypeInterceptor apply to the given type?
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    bool MatchesType(Type type);
}
{% endhighlight %}

Registered `TypeInterceptor` objects are applied against any object created by
StructureMap if the type of the new object meets the `MatchesType()` method of
`TypeInterceptor`.  You can happily use multiple `TypeInterceptors`.

Here's a fairly advanced example.  Let's say that you're using an [ Event
Aggregator](http://codebetter.com/blogs/jeremy.miller/archive/2008/01/11/build-your-own-cab-extensible-pub-sub-event-aggregator-with-generics.aspx)
pattern.  You'll typically have some sort of Event Aggregator, and a listener
interface to register with the Event Aggregator.

{% highlight csharp %}
public interface IEventListener<T>
{
    void ProcessEvent(T @event);
}

public interface IEventAggregator
{
    void RegisterListener<T>(IEventListener<T> listener);
    void PublishEvent<T>(T @event);
}
{% endhighlight %}

Let's say that anytime an object of any sort (Presenter, Controller, View, who
knows what) is created by StructureMap, we want to see if that object implements
any sort of `IEventListener<T>` interface, and if it does, register that
object as a listener with the `IEventAggregator`.  Here's the custom
`TypeInterceptor` that does just this (most of the code is actually just
massaging the generic types, but I wanted a nontrivial example):

{% highlight csharp %}
public class ListenerInterceptor : TypeInterceptor
{
    public object Process(object target, IContext context)
    {
        // Assuming that "target" is an implementation of IEventListener<T>,
        // we'll do a little bit of generics sleight of hand
        // to register "target" with IEventAggregator
        var eventType = target.GetType().FindInterfaceThatCloses(typeof (IEventListener<>)).GetGenericArguments()[0];
        var type = typeof (Registration<>).MakeGenericType(eventType);
        Registration registration = (Registration) Activator.CreateInstance(type);
        registration.RegisterListener(context, target);

        // we didn't change the target object, so just return it
        return target;
    }
  
    public bool MatchesType(Type type)
    {
        // ImplementsInterfaceTemplate is an Extension method in the
        // StructureMap namespace that basically says:
        // does this type implement any closed type of the open template type?
        return type.ImplementsInterfaceTemplate(typeof (IEventListener<>));
    }

    // The inner type and interface is just a little trick to
    // grease the generic wheels
    public interface Registration
    {
        void RegisterListener(IContext context, object listener);
    }

    public class Registration<T> : Registration
    {
        public void RegisterListener(IContext context, object listener)
        {
            var aggregator = context.GetInstance<IEventAggregator>();
            aggregator.RegisterListener<T>((IEventListener<T>) listener);
        }
    }
}
{% endhighlight %}

Finally, you can register the new `ListenerInterceptor` like this:

{% highlight csharp %}
public class ListeningRegistry : Registry
{
    public ListeningRegistry()
    {
        RegisterInterceptor(new ListenerInterceptor());
    }
}
{% endhighlight %}

By the way, this is cooked up sample code.  Don't dream for a second that it'll
work without some testing.

