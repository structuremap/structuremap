<!--Title:Working with Primitive Types-->

StructureMap treats simple types like strings, numbers of any kind, enumerations, and dates as _primitive_
types that are completely exempt from <[linkto:the-container/auto-wiring;title=auto wiring]> -- meaning that any 
constructor or setter dependencies on these types must be supplied as <[linkto:registration/inline-dependencies;title=inline dependencies]>.

To make this concrete, if you ask StructureMap to build a concrete type that has dependencies on simple types without
like this example, StructureMap will throw an exception telling you that it cannot build the instance:

<[sample:GuyWithNoDefaultName]>

Part of the exception message thrown in the unit test shown above is the erroneous <[linkto:diagnostics/build-plans;title=build plan]>
showing you that the `name` parameter has to be defined:

<pre>
	new GuyWithNoDefaultName(String name)
	  ┗ String name = Required primitive dependency is not explicitly defined
</pre>

We can build `GuyWithNoDefaultName` by supplying a value for `name` as I did in the following
sample:

<[sample:GuyWithNoDefaultName-explicit-argument]>

See <[linkto:registration/policies]> for an example of using a constructor policy to set a dependency
on a "connectionString" argument in a conventional way.

## Default Values

As a new feature in the 4.0 release, StructureMap can finally take advantage of default parameter arguments to
derive the values for a primitive argument (or setter value) while still allowing you to explicitly define
that parameter or setter value:

<[sample:GuyWithName-defaults]>


