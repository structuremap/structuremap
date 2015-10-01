<!--Title: Validating Container Configuration-->
<!--Url: validating-container-configuration-->

To find any potential holes in your StructureMap configuration like missing dependencies, unclear defaults of plugin types, validation errors, or just plain build errors, you can use this method below:

<[sample:container.AssertConfigurationIsValid]>

Running this method will walk over every single registration in your `Container` and:

1. Try to create a build plan for that Instance that will flush out any problems with missing dependencies or invalid inline configuration
1. Try to build every single configured Instance
1. Call any methods on built objects decorated with the `[ValidationMethod]` attribute to perform environment tests. See <[linkto:diagnostics/environment-tests]> for more information on this usage.

If StructureMap encounters any errors of any kind during this method, it will throw an exception summarizing **all** of the problems that it encountered. That output will look something like:

<pre>

	StructureMap.StructureMapConfigurationException
	StructureMap Failures:  1 Build/Configuration Failures and 0 Validation Errors
	
	Profile 'DEFAULT'
	
	-----------------------------------------------------------------------------------------------------
	Build Error on Instance 'StructureMap.Testing.Diagnostics.NamedWidget'
	    for PluginType StructureMap.Testing.Diagnostics.IWidget
	
	Unable to create a build plan for concrete type StructureMap.Testing.Diagnostics.NamedWidget
	new NamedWidget(String name)
	  ┗ String name = Required primitive dependency is not explicitly defined

</pre>

## Standin Dependencies for Runtime Dependencies

If you take advantage of StructureMap's ability to supply <[linkto:resolving/passing-arguments-at-runtime;title=arguments at runtime]>, you may need to add fake or stubbed _stand in_ services to StructureMap just to satisfy dependencies while calling `Container.AssertConfigurationIsValid()`. For example, in [FubuMVC](http://github.com/DarthFubuMVC/fubumvc) we supply several services representing an HTTP request as explicit arguments. In order to use the container validation, we have to register stand in services in the main container.