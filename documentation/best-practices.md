<!--Title: Best Practices-->
<!--Url: best-practices-->

All of these recommendations are the opinions and sole responsiblity of one [Jeremy D. Miller](http://jeremydmiller.com). The single best advice I can give you 
about using StructureMap is to avoid being too clever with your usage.


## Use Nested Containers for Per Transaction/Request/Message Scoping

In idiomatic StructureMap usage, I strongly recommend using a <[linkto:the-container/nested-containers;title=nested container]> for short lived operations like:

1. An HTTP request
1. Handling a single service bus message
1. Short lived transactions


## Avoid the Legacy ASP.Net Lifecycles

To repeat the section above, I strongly recommend using nested containers instead of the legacy ASP.Net lifecycles to scope
services per HTTP request.


## Avoid Container.Configure() on the Root Container

Unfortunately, the `<[linkto:registration/changing-configuration-at-runtime;Container.Configure()]>` method is potentially an expensive operation
and can result in some container-wide locks if you change any kind of interception or construction policy. Avoid this mechanism at runtime. If you need
to inject services at runtime, try to do those overrides in an isolated <[linkto:the-container/nested-containers;title=nested container]> for that
particular request or transaction to avoid hitting any kind of shared lock.


## Avoid `Container.TryGetInstance()` 

Use the container to resolve your dependency relationships or don't. In my opinion, using `TryGetInstance()` results in unnecessary complexity in your application. My recommendation is to use _[nullo objects](https://en.wikipedia.org/wiki/Null_Object_pattern)_ as stand-ins or to use "modular registration" strategies instead (See <[linkto:registration/fallback-services]> and <[linkto:registration/clear-or-replace]> for more information). The ASP.Net team requires this usage for all of their IoC container integrations and you can't fight city hall, so StructureMap 4.0 includes some performance optimizations specifically for the heavy `TryGetInstance()` usage that I never anticipated.

## Favor Constructor Injection over Setter Injection

I believe that constructor injection is less error prone than setter injection and is more easily traceable later. Setter injection is occasionally easier
to use (in inheritance relationships for an example), but is mostly available in StructureMap as a workaround for code that was not built with dependency
injection in mind (or popular frameworks that were built around Spring.Net usage *cough* NServiceBus *cough*).

## Use Child Containers for Test Isolation

See <[linkto:the-container/profiles-and-child-containers]> for more information.

## Use Registry Classes

While you can completely set up a `Container` object through its constructor function and repeated calls to `Container.Configure()`, we suggest that you
express any configurations in <[linkto:registration;title=Registry objects]> for repeatable configuration later in tests and for a cleaner expression
of the configuration.

Do consider breaking your configuration across separate `Registry` objects for different subsystems if the StructureMap configuration gets complicated.


## Onion Architecture Assembly Coupling

If your choice is between making a direct dependency from one assembly to another strictly to make the StructureMap configuration easy and less error prone or using dynamic assembly loading and configuration strategies with type scanning to comply with the _Onion Architecture_, **I very strongly recommend you favor the less error prone, direct assembly dependencies**. 

## Favor Build Policies over Fancy Runtime Decision Making

While there are many powerful things you can do with StructureMap at runtime for conditional object resolution with <[linkto:the-container/working-with-the-icontext-at-build-time;title=lambdas and IContext]>, 
I recommend trying to "bake in" building decisions upfront with <[linkto:registration/policies;title=build policies]> for more efficient object resolution and so that
the container behavior is surfaced through diagnostics like the <[linkto:diagnostics/build-plans;title=build plan visualization]>.

