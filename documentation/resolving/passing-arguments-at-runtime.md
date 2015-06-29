<!--Title: Passing Arguments at Runtime-->
<!--Url: passing-arguments-at-runtime-->



Most of the time you will be using StructureMap to build objects based on pre-canned configuration established upfront, but StructureMap
also has the capability to supply dependencies by type or named parameters (if you know the name of constructor arguments or setter property names) to the Container at runtime using the `IContainer.With()` methods.

Why would you use this? Here are a few examples from my own usage over the years:

1. In adhoc code, retrieve some service but override a `connectionString` constructor argument to connect to a different database
1. If you were to use StructureMap as a configuration intermediary, it becomes common to use this mechanism to swap out configuration on the fly
1. Inject an Entity object into a service (uncommon now, but I did this on several systems years ago)
1. Inject something contextual that can only be built at runtime like an ASP.Net HttpContext into a pre-configured object graph

Now, for some samples. Let's say that we have some classes like so:

<[sample:explicit-domain]>

and unless stated otherwise in a sample, a `Container` configured like so:

<[sample:explicit-arg-container]>


## Passing Named Arguments

The original usage of explicit arguments was to replace primitive arguments to constructor functions like this sample:

<[sample:explicit-named-arguments]>

The canonical usage is overriding file paths, database connection string, or urls.

## Passing Arguments with a Fluent Interface

You can invoke the explicit argument passing as a fluent interface starting with
`IContainer.With()` like the following sample:

<[sample:explicit-fluent-interface]>

## Using a Nested Closure

If you dislike fluent interfaces or want to pass in a *lot* of dependencies,
the nested closure syntax might be more usable:

<[sample:explicit-defaults-with-nested-closure]>

## Using the ExplicitArguments object

Finally, you can also pass an object of type `ExplicitArguments` directly to an overload
of the `IContainer.GetInstance()` method:

<[sample:explicit-use-explicit-args]>





