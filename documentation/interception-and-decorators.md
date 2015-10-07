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

## Defining Activation Interceptors

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



## Activate all Instances of a Certain Type



<[sample:Activateable]>


<[sample:AWidget-is-Activateable]>

<[sample:activate_by_action]>

