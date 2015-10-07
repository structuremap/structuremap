<!--Title: Interception and Decorators-->
<!--Url: interception-and-decorators-->

All of the samples from this topic are part of the [user acceptance tests](https://github.com/structuremap/structuremap/blob/master/src/StructureMap.Testing/Acceptance/interception_acceptance_tests.cs) in the main codebase.

Improving the interception facilities and the means of applying [decorators](https://en.wikipedia.org/wiki/Decorator_pattern) during object construction was one of the primary
goals of the big 3.0 release and is significantly different than the older 2.5/2.6 model. 

Interception in StructureMap really comes
in two basic flavors:

1. _Activates_ - Do some action on or with an object just created or resolved by StructureMap
1. _Decorates_ - Wrap (or optionally replace) an object just created with either a dynamic proxy or some sort of decorator

Interceptors can be configured explicitly on a single `Instance` registration, on all registrations to a `PluginFamily`, or conventionally
to any concrete type implementing an interface, inheriting from a certain base class, or by some sort of user-supplied
criteria.

Any type of `Instance` can be intercepted, meaning that even object literal values supplied to StructureMap at registration can be
intercepted when they are resolved as dependencies or through service location calls.

See the <[linkto:glossary]> for a refresher on terms like `Instance` and `PluginFamily`.

Also see <[linkto:the-container/working-with-the-icontext-at-build-time]> for more information about using the `Container` state within interception.

## Activation Interceptors

For right now, all activation interceptors are either using or subclassing the [ActivatorInterceptor&lt;T&gt;](https://github.com/structuremap/structuremap/blob/master/src/StructureMap/Building/Interception/ActivatorInterceptor.cs) class.

This class has two constructor functions that interest us:

To create an activator interceptor that acts on an object that can be cast to type `T`:
<[sample:ActivatorInterceptor-by-action-T]>


To create an activator interceptor that acts on an object that can be cast to type `T` and
also uses the <[linkto:the-container/working-with-the-icontext-at-build-time;title=IContext]> service supplied by StructureMap itself.
<[sample:Activator-by-action-T-and-IContext]>

In both cases, the `description` is optional and is only used for diagnostic purposes in the <[linkto:diagnostics/build-plans;title=build plan visualization]>. If 
omitted, StructureMap tries to do a `ToString()` on the Expression for the description and that frequently suffices to understand what's going on in the build plan.

**Please note that the Lambda supplied to `ActivatorInterceptor<T>` must be a .Net Expression so cannot be a multi-line Lambda.**
You can get around this limitation for more complex activation needs by simply making a wrapping method and using that
to express the activation.


## Decorators

To demonstrate a decorator in action, say that we have an interface called `IWidget`, and when we build any instance
of `IWidget` we want those objects decorated by another type of `IWidget` that I clumsily named `WidgetHolder` in the
acceptance tests:

<[sample:WidgetHolder]>

Now, to see the decorator mechanism in action:

<[sample:decorator-by-type-example]>

In effect, doing a decorator this way has the same effect (and build plan) as:

<[sample:simple-decorator-equivalent]>

## Custom Decorator Interceptors

The simplest usage is to just declare a type that will be the decorating type like we did above, but if you need some
other mechanism for decorators like runtime AOP interception or you want to build the decorating object yourself, StructureMap
provides the `FuncInterceptor<T>` type where `T` is the type you want to decorate.

These objects can be created in two ways, by a user-supplied `Expression<Func<T, T>>` and optional description:
<[sample:FuncInterceptor-by-expression]>

and by a user-supplied `Expression<Func<IContext, T, T>>` and optional description.
<[sample:FuncInterceptor-by-expression-and-icontext]>

In both cases, the `description` field is only used for diagnostic purposes.

## Interception Policies

The <[linkto:registration/registry-dsl]> includes shorthand methods for the most common ways of configuring decorators
and activators by an individual `Instance` or by matching on implementing types. For more customized interception policies
that don't fit these mechanisms, StructureMap allows you to directly define an interception policy with a class
implementing this interface below:

<[sample:IInterceptorPolicy]>

For a simple example, let's say that we want to decorate any `IWidget` object with the
`WidgetHolder` class from earlier. We could build a small custom interceptor policy 
like this one:

<[sample:CustomInterception]>

To use this custom interception policy, use the `Policies.Interceptor()` methods like this example:

<[sample:use_a_custom_interception_policy]>


As a helper for creating your own interception policies, you can also use the `InterceptorPolicy<T>` base class
to conventionally apply some sort of `IInterceptor` to any number of `Instance's`:

<[sample:InterceptorPolicy<T>]>

Here's an example of `InterceptorPolicy<T>` in usage from the acceptance tests:

<[sample:InterceptorPolicy<T>-in-action]>

Some quick things to note:
1. For decorator interceptors, `InterceptorPolicy<T>` will only apply if the `pluginType` matches `T`
1. For activation interceptors, `InterceptorPolicy<T>` will apply to any concrete type returned by an `Instance` that can be
cast to `T`


## Apply Activation Interception by Type

Let's say that in your system you have a marker interface or in this case an abstract class that exposes a single
`Activate()` method to start up stateful, long-running services created within your container:

<[sample:Activateable]>

An implementation of `Activateable` from StructureMap's unit tests is shown below:

<[sample:AWidget-is-Activateable]>

If you decide that you'd like StructureMap to call the `Activate()` method on any object it creates as
part of its object creation and resolution process, we can register an interception policy in a `Registry`
like this:

<[sample:activate_by_action]>

There are several overloads of `OnCreationForAll()` covering cases with and without `IContext`


## Apply Decoration across a Plugin Type

As shown above, you can use the `Registry.For<T>().DecorateAllWith<TDecorator>()` to apply decorators to all `Instance's`
registered to a `Plugin Type`:

<[sample:decorate-plugin-type-with-type]>

There are also several other overloads of `DecorateAllWith()` for user supplied expressions, filters, and descriptions. See the
[acceptance tests for interception](https://github.com/structuremap/structuremap/blob/master/src/StructureMap.Testing/Acceptance/interception_acceptance_tests.cs) in the StructureMap codebase for many more sample usages.



## Add Interception to a Single Instance

You can also define interceptors directly to individual `Instance's` inside of a StructureMap `Registry` using the
`OnCreation()` and `DecorateWith` methods or the more generic `Instance.AddInterceptor()` method. Here is some sample
usage from StructureMap's unit tests on interception:

<[sample:interceptors-by-instance]>