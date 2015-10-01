<!--Title: Build Plans-->
<!--Url: build-plans-->

StructureMap 3.0 introduced the concept of the "Build Plan." The build plan is a textual representation of exactly how StructureMap will build a given Instance, including:

1. What constructor function, if any, StructureMap is going to use for a concrete type
1. How it is resolving the constructor dependencies in a recursive fashion
1. Which setter properties for a concrete type receive dependencies and how those setter dependencies are built
1. The lifecycle used for the Instance
1. All interceptors applied to the Instance
1. Explicitly configured inline dependencies
1. Description of any Lambda or `Func<T>` that is used to construct the Instance object

To retrieve the build plan for a configured Instance, use the queryable `Container.Model` to find the configured Instance and call `DescribeBuildPlan(int maxDepth)` to get the textual report as shown in this sample below:

<[sample:build-plan-deep-for-default]>

The result of the code above is this textual representation of how StructureMap will build and/or resolve the default of the `IDevice` plugin type:

<pre>
PluginType: StructureMap.Testing.Diagnostics.IDevice
Lifecycle: Transient
Decorator --> FuncInterceptor of StructureMap.Testing.Diagnostics.IDevice: new DeviceDecorator(IDevice)
Decorator --> Decorator of type StructureMap.Testing.Diagnostics.CrazyDecorator
    new CrazyDecorator(IDevice, IEngine, IFoo)
      ┣ IDevice = The inner IDevice
      ┃ ┗ new DefaultDevice()
      ┣ IEngine = **Default**
      ┗ IFoo = **Default**
      
</pre>

For another example, you can retrieve the build plan for a named instance of a certain build plan with this code:

<[sample:build-plan-by-name]>

See <[linkto:diagnostics/using-the-container-model]> for more information on using `Container.Model`.

You can find many more examples of finding the build plan description from `Container.Model` from the [unit tests in the StructureMap codebase](https://github.com/structuremap/structuremap/blob/master/src/StructureMap.Testing/Diagnostics/BuildPlanVisualizationSmokeTester.cs).

