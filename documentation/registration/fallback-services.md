<!--Title: Fallback Services-->
<!--Url: fallback-services-->

The following is a technique that was stolen from [FubuMVC](http://github.com/darthfubumvc/fubumvc) where we used
the idea of default or "fallback" registrations to make it mechanically simple for the core framework to declare
the default service registrations in the `Container` for what FubuMVC needed while allowing applications to happily
override specific registrations **without having to worry about the order in which the registrations were done**.

To see this in practice, say you have an application that will support client specific modularity that might allow
business clients to override the base StructureMap registrations. This is a perfect use case for
defining the application defaults with `UseIfNone()` as shown in this example below:

<[sample:fallback_registrations]>


In application usage, you would add the default `UseIfNone()` registrations, and optionally pick
up additional extension `Registry` objects from extension assemblies as shown in this example:

<[sample:use-if-none-modularity]>


