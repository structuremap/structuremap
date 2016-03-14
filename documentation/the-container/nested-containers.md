<!--Title: Nested Containers (Per Request/Transaction)-->
<!--Url: nested-containers-->


<div class="alert alert-info" role="alert"><i>Nested Containers</i> are <b>not</b> interchangeable with <i>Child Containers</i>. See <linkto:the-container/profiles-and-child-containers]> for more information on child containers.</div>

_Nested Container's_ are a powerful feature in StructureMap for service resolution and clean object disposal in the 
context of short lived operations. Nested Container's were introduced in version 2.6, but greatly improved in both performance (100X reduction in the time to create a nested container in a large application) and _ahem_ [lifecycle
mechanics](http://github.com/structuremap/structuremap/issues/3) as a major goal of the 3.0 release.

## History

The original use case and impetus for building this feature was a simplistic message handling system that dequeued
messages from a Sql Server table (please forget for a second the wisdom of using Sql Server as a queueing system), deserialized
the contents into a .Net object, then created the proper handler object for that message type and executed that handler -- all within
a single transaction. What we wanted at the time was a way to track and clean up all `IDisposable` objects created during the lifespan of each
transaction. We also wanted a new type of object lifecycle where objects like the [NHibernate ISession](http://elliottjorgensen.com/nhibernate-api-ref/NHibernate/ISession.html) would be shared by every object 
created during the lifetime of the nested container -- even if the `ISession` was resolved lazily after the intial 
resolution of the message handler. The result was what is now the _nested container_ feature
of StructureMap. 

## Why Nested Containers over HttpContext or ThreadLocal Scoping?

Why not just use `HttpContext` based lifecycles like we've always done in the past? Because `HttpContext` is not supported by the any
type of [OWIN](http://www.strathweb.com/2013/05/the-future-is-now-owin-and-multi-hosting-asp-net-web-applications/) web host and will not be a part of ASP.Net vNext. Using a Nested Container per HTTP request is a better, lighterweight way
to scope services to an HTTP request without coupling your code to what will soon be legacy ASP.Net runtime code.

## Who uses it?

At the time of this document, Nested Container's per HTTP request are supported by frameworks like [FubuMVC](http://fubuworld.com/fubumvc),
ASP.Net MVC through the [StructureMap.MVC5](https://www.nuget.org/packages/StructureMap.MVC5/) nuget package, and Web API with the [StructureMap.WebApi2](https://www.nuget.org/packages/StructureMap.WebApi2/) nuget. Several service bus frameworks also use a StructureMap nested container per message invocation including [FubuTransportation](http://fubuworld.com/fubutransportation),
[MassTransit](http://masstransit-project.com), and [NServiceBus](http://particular.net/nservicebus).




## Creation
Creating a nested container is as simple as calling the `IContainer.GetNestedContainer()` method as shown below:
<[sample:nested-creation]>





## Lifecycle Rules

While StructureMap supports several object instance lifecycles out of the box, in idiomatic usage of StructureMap the only common lifecyles are:

1. `Transient` - The default lifecycle. A new object is created for a configured Instance on each request to the container
1. `Singleton` - One instance is constructed and used over the entire Container lifetime

In the context of a Nested Container however, the `Transient` scoping now applies to the Nested Container itself:

<[sample:nested-transients]>

`Instance's` scoped to anything but `Transient` or `AlwaysUnique` are resolved as normal, but **through the parent container**:

<[sample:nested-singletons]>

See <linkto:object-lifecycle]> for more information on supported object lifecycles.




## Overriding Services from the Parent

A nested container is a new Container object that still retains access to the parent container that created it so that it can
efficiently share registrations, policies, and cached _build plans_. You can, however, register services into the nested container that override the parent container.

<div class="alert alert-info" role="alert">There is no need to recreate registrations from the parent container
into the nested container and doing so makes StructureMap work less efficiently.</div>

The [FubuMVC web framework](http://fubuworld.com/fubumvc) uses a nested container per HTTP request. During an HTTP request, FubuMVC injects services
for the current HTTP request and response to a nested container before creating the actual services that will handle the request. The
FubuMVC mechanics are conceptually similar to this code sample:

<[sample:nested-overriding]>

<div class="alert alert-info" role="alert">The StructureMap team strongly cautions against altering the configuration of the main application container, but overriding services in a nested container is useful (but please upgrade to at least version 3.1.2) and should not incur any problems with locking across threads.</div>

When handling requests for new services, a nested container first checks its own configuration if it has its own explicit registration for the request. If the nested container does have an explicit registration, it uses that registration. Otherwise, a nested container will attempt to build
an object using the registered configuration of its parent container.

<div class="alert alert-info" role="alert">While the 3.0 version of nested containers acts like a <a href="http://en.wikipedia.org/wiki/Chain-of-responsibility_pattern">Chain of Responsibility</a> pattern to apply its own overrides, the earlier version
 of StructureMap made a complete copy of the underlying Container configuration. This crime against computer science was removed in 3.0 and hence, the 100X 
 improvement in the time it takes StructureMap to create a nested container.</div>




## Lazy Resolution

Nested container object lifecycles equally apply to objects resolved lazily with
either `Lazy<T>`, `Func<T>`, or `Func<string, T>` as shown below:

<[sample:nested-func-lazy-and-container-resolution]>

<div class="alert alert-info" role="alert">Do note that the <code>IContainer</code> object injected above was the nested container that
created the <code>FooHandler</code> object. If you do want to use service location within an object, just take in
<code>IContainer</code> as a constructor dependency and you will always get the correctly scoped <code>IContainer</code>.</div>



## Profiles

You can created nested containers from profile containers as shown in the sample below:

<sample:nested-profiles]>

See <[linkto:the-container/profiles-and-child-containers]> for more information about using profiles.

## Disposing Services

As stated above, disposing a nested container will also dispose all objects created with the default _Transient_ lifecycle by the
nested container that implement the `IDisposable` interface. That behavior is demonstrated
below:

<[sample:nested-disposal]>

For the sake of clarity, the classes used in the sample above are:

<[sample:nested-colors]>






