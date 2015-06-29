<!--Title: Supported Lifecycles-->
<!--Url: supported-lifecycles-->

Out of the box, the core StructureMap assembly supports these lifecycles:

* Transient -- The default lifecycle.  A new object is created for each logical request to resolve an object graph from the container.  
* Singleton -- Only one object instance will be created for the main Container 
* AlwaysUnique -- A new object instance is created every time, even within the same object graph
* ThreadLocal -- Only one object instance will be created for the currently executing Thread


## Transient

Older versions of StructureMap referred to _Transient_ as _PerRequest_, which might be a more accurate reflection of how this lifecycle behaves but 
causes some confusion with ASP.Net HTTP scoping.  The easiest way to think of _Transient_ is that a single object instance will be created for each top level
call to `Container.GetInstance()` (or any other object resolution method on the `IContainer` interface). _Transient_ objects resolved from a nested container, _Transient's_ are scoped to the lifecycle
of the nested container itself. See <[linkto:the-container/nested-containers]> for more information.

<div class="alert alert-info" role="alert">Transient lifecycle objects are <b>only</b> tracked and disposed if created by nested containers. If you resolve a transient object from the main application container,
it will not track the object created. While that behavior avoids the manual <i>Release()</i> method and potential memory leak found in other IoC containers like Windsor, it
puts the onus for disposing those objects on the user. The StructureMap team strongly recommends using nested containers for short
lived operations if disposing transient objects created by the Container is important.</div>

The following unit test demonstrates how _Transient_ lifecycles work in both root and nested containers. 

<[sample:how-transient-works]>

Also note that a transient dependency will
be created exactly once in an object graph resolved from `IContainer.GetInstance(Type)`. Imagine that you are building an
object graph with various objects that all need to apply some work to a shared [unit of work](http://msdn.microsoft.com/en-us/magazine/dd882510.aspx) object (think NHibernate's ISession, Entity Framework's DbContext, RavenDb's IDocumentSession):

<[sample:transient-are-shared-within-a-graph]>



## AlwaysUnique

Very simply, using the _AlwaysUnique_ means that a new object instance will be created every single time a configured Instance is either requested
from a Container or as a dependency to another object. The _AlwaysUnique_ lifecycle is a "fire and forget" operation as the object instances are neither tracked nor disposed by StructureMap. 

<div class="alert alert-info" role="alert">The StructureMap team strongly feels that the default lifecycle is appropriate in most cases and that the <i>unique</i>
lifecycle is mostly useful for logging services as demonstrated in the <a href="http://jeremydmiller.com/2014/08/12/structuremap-3-1/">StructureMap 3.1 announcement.</a></div>

<[sample: how-always-unique]>

## Singleton

StructureMap 3.0 fixed the [dreaded singletons with nested container's bug](https://github.com/structuremap/structuremap/issues/3) that was so problematic in 2.6. 

<[sample:singleton-in-action]>

Do note that objects created as the singleton scope will be disposed when the Container is disposed if they
implement the `IDisposable` interface:

<[sample:transient-are-shared-within-a-graph]>

## ThreadLocal

The ThreadLocalStorage based lifecycle is seldom used, but the easiest example of using it and explanation is the integration test:
<[sample:thread-local-storage]>



## Legacy ASP.Net Lifecycles

<div class="alert alert-info" role="alert">The StructureMap team strongly recommends against using the old ASP.Net lifecycles.  <b>Most</b> modern web frameworks in .Net will use a Nested Container per request to accomplish the same scoping in a better way.</div>

<Nuget name="StructureMap.Web" />

In addition, the StructureMap.Web package adds the legacy ASP.Net related lifecycles for:

* HttpContext
* HttpSession (requires objects to be `Serializable`, not recommended but still present for legacy code
* Hybrid -- uses ThreadLocalStorage in the absence of an active HttpContext
* HybridSession

<[sample:AspNet-Lifecycles]>

If you do use any of the HttpContext lifecycles, make sure you also do:

<[sample:clean-up-http-context]> 

at the end of your HTTP request.
