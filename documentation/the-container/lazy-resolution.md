<!--Title:Lazy Resolution-->

StructureMap has some built in functionality for "lazy" resolved dependencies, so that instead of your
application service taking a direct dependency on `IExpensiveToBuildService` that might not be necessary,
you could instead have StructureMap fulfil a dependency on `Lazy<IExpensiveToBuildService>` or `Func<IExpensiveToBuildService>`
that could be used to retrieve that expensive service only when it is needed from whatever `Container` originally created
the parent object.

Do note that the `Lazy<T>` and `Func<T>` approaches respect the lifecycle of the underlying registration rather than
automatically building a unique object instance.

Also note that `Lazy<T>` or `Func<T>` is your best (only) viable approach if you wish to have StructureMap inject bi-directional
relationships.

## Lazy&lt;T&gt;

Assuming that StructureMap either has an existing configuration for `T` or can
derive a way to build `T`, you can just declare a dependency on `Lazy<T>` like this sample:

<[sample:Lazy-in-usage]>


## Func&lt;T&gt;

Likewise, you can also declare a dependency on `Func<T>` with very similar mechanics:

<[sample:using-func-t]>

_This functionality predates the introduction of the Lazy type to .Net_

## Func&lt;string, T&gt;

Finally, you can also declare a dependency on `Func<string, T>` that will allow you to lazily
resolve a dependency of `T` by name:

<[sample:using-func-string-t]>

## Bi-relational Dependency Workaround

**StructureMap does not directly support bi-directional dependency relationships** -- but will happily tell you in an exception when
you accidentally manage to create one without cratering your AppDomain with a `StackOverflowException`.

Either `Func<T>` or `Lazy<T>` can be used as a workaround for purposeful bi-directional dependencies between types. The
following is an example of using this strategy:

<[sample:using-lazy-as-workaround-for-bidirectional-dependency]>