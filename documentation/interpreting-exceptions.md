<!--Title: Interpreting Exceptions-->
<!--Url: interpreting-exceptions-->

Improving the exception messages was one of the primary goals of the big StructureMap 3.0 release. StructureMap exceptions should
show a "StructureMap-specific stacktrace" describing where in the object graph StructureMap was when an exception occurred, while
trying hard not to obscure the actual problems. In some cases, StructureMap will append <[linkto:diagnostics;title=diagnostic views]> 
to the exception output. **If you are still using StructureMap 2.5-2.6, the better exception messages alone are a justification for upgrading.**

StructureMap throws a couple custom exceptions, all of which implement a base `StructureMapException` type:

1. `StructureMapBuildException` -- a failure was detected during an attempt to build or resolve a requested object or dependency, but outside of
   StructureMap code. This exception wraps the actual exception and adds StructureMap-specific contextual information. **Always check the inner
   exception** when this exception is thrown.
1. `StructureMapInterceptorException` -- a configured interceptor failed during object construction. See the inner exception.
1. `StructureMapBuildPlanException` -- StructureMap is missing some kind of configuration or needs more explicit registrations in order to 
   build an `Instance`. In a few cases, this exception will be thrown if StructureMap cannot create and compile a `Func` internally for the 
   `Instance.`
1. `StructureMapConfigurationException` -- thrown by the <[linkto:diagnostics/validating-container-configuration;title=container validation mechanism]>


