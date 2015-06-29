<!--Title: Changing Configuration at Runtime-->
<!--Url: changing-configuration-at-runtime-->


<div class="alert alert-info" role="alert">Use the technique shown here with caution.</div>


If you need to add or change configuration to an existing StructureMap `Container` object, you can use the `IContainer.Configure()` method
to add or change your container at runtime as shown below:

<[sample:container-configure]>

## Best Practices

First off, the best advice on this functionality is _don't use it outside of testing scenarios on the root container_. The `Configure()` method has to use a threading lock around the internal object model of a StructureMap container and can cause serious contention at runtime if you try to override services in the main application controller. Some frameworks (looking at you NancyFx) have abused this functionality quite badly in the past and the result was not pretty.

* Do favor writing configuration to StructureMap `Registry` objects, then applying that `Registry` to a container rather than repeatedly calling `Configure()`
* Do not call `Configure()` on the main application container after the initial configuration. Use nested or child containers that are not shared across threads or HTTP requests if you need to override services at runtime
* There's a potential performance hit from using `Configure()` at runtime because StructureMap has to recycle its internal _Build Plan's_ based on
the potential new configuration.
 
