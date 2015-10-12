<!--Title: Try Geting an Optional Service by Plugin Type and Name-->
<!--Url: try-geting-an-optional-service-by-plugin-type-and-name-->


Just use the `IContainer.TryGetInstance<T>(name)` or `IContainer.TryGetInstance(Type pluginType, string name)` method as shown below:

<[sample:TryGetInstanceViaNameAndGeneric_ReturnsInstance_WhenTypeFound]>


