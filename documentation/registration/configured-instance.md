<!--Title:Working with IConfiguredInstance-->

The most common way for StructureMap to build or resolve a requested object is to build a concrete type directly by calling a
public constructor function and optionally filling values in public setter properties. For this type of object construction, 
StructureMap exposes the `IConfiguredInstance` interface as a means of querying and modifying how a concrete type will be
created or resolved. While the <[linkto:registration/registry-dsl]> fluent interface provides the main way of explicitly configuring concrete type creation,
the `IConfiguredInstance` interface is meant to support <[linkto:registration/auto-registration-and-conventions;title=conventional registration]>, 
<[linkto:registration/attributes;title=configuration attributes]>, and <[linkto:registration/policies;title=construction policies]>.

<[sample:IConfiguredInstance]>


## Changing the Instance Lifecycle

You can override the lifecycle of a single `IConfiguredInstance` by calling the `LifecycleIs()` methods and either supplying a 
type of `ILifecycle` or an `ILifecycle` object. As a quick helper, there are also extension methods for common lifecycles:

<[sample:iconfiguredinstance-lifecycle]>

## Reflecting over Constructor Parameters

To find the constructor function parameters of an `IConfiguredInstance`, just use this syntax (it's just .Net Reflection):

<[sample:reflecting-over-parameters]>

**The <[linkto:registration/constructor-selection;title=constructor function selection]> process takes place as the very first step in creating a <[linkto:diagnostics/build-plans;title=build plan]> and will be
available in any kind of <[linkto:registration/policies;title=construction policy]> or <[linkto:registration/attributes;title=configuration attribute]> on
parameters or properties.**



## Reflecting over Setter Properties

There's a helper extension method off of `IConfiguredInstance' for finding all of the settable properties
that StructureMap can work with as shown below:

<[sample:iconfiguredinstance-getsettableproperties]>


## Working with Dependencies

The `IConfiguredInstance.Dependencies` property is a collection of `Argument` objects that model inline dependencies. A
single _Argument_ can refer to a public property or the parameter in a constructor function and consists of:

1. Type - the dependency type that would match a property or parameter argument
1. Name - matches the name of a property or parameter argument
1. Dependency - either an object or value of the dependency type or an Instance object that can be used to build the dependency

When StructureMap determines a <[linkto:diagnostics/build-plans;title=build plan]> for a concrete type, it reflects over all the 
parameters in the chosen constructor function and then the settable properties looking for any explicitly configured
dependencies by searching in order for:

1. An exact match by dependency type and name
1. A partial match by dependency type only
1. A partial match by name only

For primitive arguments like strings or numbers, the logic is to search first by name, then by type. All searching is done in
the order that the `Argument` objects are registered, so do watch the order in which you add arguments. There is a method to
insert new arguments at the front of the list if you need to do any kind of overrides of previous behavior.

There are several `Add()` overloads on `IConfiguredInstance.Dependencies` to add dependencies, or you can use the two helper
methods for constructor parameters and setter properties shown in the following sections.



## Add a Dependency for a Setter Property

If you already have a `PropertyInfo` for the concrete type (like you might in a policy or attribute usage) and you want to register an inline dependency, there is the
`Dependencies.AddForProperty()` method as a convenience. For the actual value of the dependency, it needs to either be an object
that can be cast to the property type or an Instance object that returns a type that can be cast to the property type.

With a value:

<[sample:add-dependency-by-property-info]>


With an Instance for the dependency value:

<[sample:add-dependency-by-property-info-with-instance]>


## Add a Dependency for a Constructor Parameter

Likewise, you can add a dependency for a specific constructor parameter as either the actual value or an Instance object with the `AddForConstructorParameter` helper method:

<[sample:add-dependency-by-constructor-parameter]>


## Adding Interceptors

You can add interceptors directly to a single `IConfiguredInstance` with code like this:

<[sample:add-interceptor-to-iconfigured-instance]>

See <[linkto:interception-and-decorators]> for more information.