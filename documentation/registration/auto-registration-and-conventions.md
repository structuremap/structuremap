<!--Title: Auto-Registration and Conventions-->
<!--Url: auto-registration-and-conventions-->


<div class="alert alert-info" role="alert">The type scanning facilities were completely overhauled and greatly optimized for performance as part of the StructureMap 4.0 release. The mechanism for custom registration conventions is somewhat different from 2.5-3.* to 4.0.</div>


StructureMap has rich support for registering types by scanning assemblies and applying conventional registrations.
Between scanning and default conventions, configurations are often just a few
lines.


Also see <[linkto:diagnostics/type-scanning]> for help in understanding the assembly scanning behavior in your system.


## Registry.Scan()

Assembly scanning operations are defined by the `Registry.Scan()` method demonstrated below:

<[sample:BasicScanning]>

Please note (because I've been asked this several times over the years) that each call to `Registry.Scan()` is an entirely atomic operation that has no impact on previous or subsequent calls.

Any given call to `Registry.Scan()` consists of three different things:

1. One or more assemblies to scan for types
1. One or more registration conventions
1. Optionally, set filters to only include certain types or exclude other types from being processed by the scanning operation



## Scan the Calling Assembly

One of the easiest ways to register types is by scanning the assembly your
registry is placed in. 

**Note** if you have other registries, StructureMap will not automatically
find them.

<[sample:scan-calling-assembly]>

**Note that this method is an extension method in the StructureMap.Net4 assembly and cannot be used
if you target PCL compliance.**

## Scan for Registries

StructureMap can automatically include other registries with the`LookForRegistries`
method.


<[sample:scan-for-registries]>

**As of 4.0, this operation is now recursive and StructureMap has always been idempotent about adding Registry types**

## Search for Assemblies on the File System

StructureMap provides facilities for registering types by finding assemblies in the application bin path:

<[sample:scan-filesystem]>

Do be aware that while this technique is very powerful for extensibility, it's been extremely problematic for
some folks in the past. The StructureMap team's recommendation for using this feature is to:

1. Make sure you have some kind of filter on the assemblies scanned for performance and predictability reasons. Either a naming convention or filter
   by an assembly attribute to narrow where StructureMap looks
1. Get familiar with the new <[linkto:diagnostics/type-scanning;title=type scanning diagnostics]> introduced in 4.0;-)


Behind the scenes, StructureMap is using the `Assembly.GetExportedTypes()` method from the .Net CLR to find types and this
mechanism is **very** sensitive to missing dependencies. Again, thanks to the new <[linkto:diagnostics/type-scanning;title=type scanning diagnostics]>,
you now have some visibility into assembly loading failures that used to be silently swallowed internally.

## Excluding Types

StructureMap also makes it easy to exclude types, either individually or by namespace.
The following examples also show how StructureMap can register an assembly by providing
a type within that assembly.

Excluding additional types or namespaces is as easy as calling the corresponding method
again.

<[sample:scan-exclusions]>

## Custom Registration Conventions

It's just not possible (or desirable) for StructureMap to include every possible type of auto registration
convention users might want, but that's okay because StructureMap allows you to create and use your own
conventions through the `IRegistrationConvention` interface:

<[sample:IRegistrationConvention]>

Let's say that you'd like a custom convention that just registers a concrete type against all the interfaces
that it implements. You could then build a custom `IRegistrationConvention` class like the following example: 

<[sample:custom-registration-convention]>

## The Default ISomething/Something Convention

The "default" convention simply tries to connect concrete classes to interfaces using
the I[Something]/[Something] naming convention as shown in this sample:

<[sample:WithDefaultConventions]>

_The StructureMap team contains some VB6 veterans who hate Hungarian Notation, but can't shake the "I" nomenclature._

## Registering the Single Implementation of an Interface

To tell StructureMap to automatically register any interface that only has one concrete implementation, use this method:

<[sample:SingleImplementationsOfInterface]>

## Register all Concrete Types of an Interface

To add all concrete types that can be cast to a named plugin type, use this syntax:

<[sample:register-all-types-implementing]>

**Note, "T" does not have to be an interface, it's all based on the ability to cast a concrete type to the "T"**


## Generic Types

See <[linkto:generics]> for an example of using the `ConnectImplementationsToTypesClosing`
mechanism for generic types.


## Register Concrete Types against the First Interface

The last built in registration convention is a mechanism to register all concrete types
that implement at least one interface against the first interface that they implement.

<[sample:using-RegisterConcreteTypesAgainstTheFirstInterface]>


