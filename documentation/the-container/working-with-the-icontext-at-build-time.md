<!--Title: Working with the IContext at Build Time-->
<!--Url: working-with-the-icontext-at-build-time-->


**The `IContext` usage shown in this topic is certainly not going away in future versions of StructureMap, but if at all possible, the StructureMap team
strongly recommends using <[linkto:registration/policies]> to accomplish customized object building instead of relying
on conditional logic using `IContext` at runtime.**

The `IContext` interface is what StructureMap uses to expose the current state of the internal "build session" for
the current object creation operation for usage in user-supplied <[linkto:the-container/lambdas;title=lambda builders]> or 
<[linkto:interception-and-decorators;title=interceptors]>. `IContext` allows you to:

1. Retrieve other services from the current `Container` to use in a Lambda
1. <[linkto:the-container;title="BuildUp" objects with setter dependencies]>
1. Retrieve all the objects created inside the current build session that could be cast to a certain interface or base class
1. If an object is being built as a dependency to another object, see the parent type
1. See the root type of the top level object that StructureMap is building in this request

The entire `IContext` interface is shown below:

<[sample:IContext]>



## Example 1: Contextual Logging

All of [the sample code in this example is in Github](https://github.com/structuremap/structuremap/blob/master/src/StructureMap.Testing/Acceptance/contextual_building.cs).

One of the canonical examples of using `IContext` is the integration of logging frameworks like NLog or Log4net
that allow you to create logging policies by concrete type. With these tools, you generally have a static class (boo!)
where you ask for the proper logging service for a type of object like this one:

<[sample:LoggerFactory]>

Now, say you would want to have the proper `Logger` injected into every object that StructureMap creates that depends on `Logger` matching
the concrete type of the object being created. That registration is shown below in a unit test from StructureMap's codebase:

<[sample:Logger-contextual-building]>

Just for fun, here's the equivalent with the new <[linkto:registration/policies;title=construction policies from 4.0]>:

<[sample:LoggerConvention]>

The `LoggerConvention` way of accomplishing the logging integration is technically more code and possibly harder to understand,
but it's significantly more efficient at runtime because the decision about which `Logger` to use is only done once upfront. 
The conventional approach should also be more evident in StructureMap diagnostics.



## Example 2: NHibernate Integration

_It's been several years since I used NHibernate, so I might not have the technical details exactly right here:)_

If you're developing a system with [NHibernate](http://nhibernate.info) managed by StructureMap, you will frequently need to inject
NHibernate's `ISession` service into your concrete classes. There's a little catch though, to create an `ISession` object
you have to use NHibernate's `ISessionFactory` service:

<[sample:nhibernate-isession-factory]>

Sidestepping the issue of how to build an `ISessionFactory`, here's a possible way to enable a StructureMap
`Container` to build and resolve dependencies of `ISession` with a <[linkto:the-container/lambdas;title=lambda builder]>:

<[sample:SessionFactoryRegistry]>

