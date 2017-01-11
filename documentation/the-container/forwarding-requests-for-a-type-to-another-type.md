<!--Title: Registering a Single Instance Against Multiple PluginTypes-->
<!--Url: forwarding-requests-for-a-type-to-another-type-->

It's not uncommon to want a single instance registration in StructureMap to be mapped to multiple
plugin type interfaces, especially if you have a class that implements multiple [role interfaces](http://martinfowler.com/bliki/RoleInterface.html)
to support different consumers. 

As a simplified example, let's say that we have a class named `StatefulCache` that will govern some kind
of application wide state. Some consumers will only need to read or query data, so we'll create an `IReader`
interface for that role. Other consumers will only make updates to the `StatefulCache`, so we'll also have
an `IWriter` interface as shown below: 

<[sample:forwarding-sample-types]>

What we need from StructureMap is a way to make all requests to either `IReader` and `IWriter`
resolve to the same singleton object instance of `StatefulCache`.

StructureMap provides the `Forward<TFrom, TTo>()` mechanism for exactly this purpose:

<[sample:forwarding-in-action]>

And because the syntax has consistently been confusing to users (including me even though I wrote it),
here's how to effect the same registration with <[linkto:the-container/lambdas;title=lambda registrations]>:

<[sample:forward-without-forward]>


