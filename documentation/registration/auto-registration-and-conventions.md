<!--Title: Auto-Registration and Conventions-->
<!--Url: auto-registration-and-conventions-->


StructureMap has rich support for registering types by scanning assemblies.
Between scanning and default conventions, configurations are often just a few
lines.

## Scan the Calling Assembly

One of the easiest ways to register types is by scanning the assembly your
registry is placed in. 

**Note** if you have other registries, StructureMap will not automatically
find them.



<[sample:scan-calling-assembly]>

## Scan for Registries

StructureMap can automatically include other registries with the`LookForRegistries`
method.


<[sample:scan-for-registries]>

## Scan the File System

StructureMap provides facilities for registering types by path.



<[sample:scan-filesystem]>

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


