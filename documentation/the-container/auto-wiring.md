<!--Title: Auto Wiring-->
<!--Url: auto-wiring-->


The best way to use an IoC container is to allow "Auto Wiring" to do most of the work for you.  IoC Containers like StructureMap are an infrastructure concern, and as such, should be isolated from as much of your code as possible.  Before examining Auto Wiring in depth, let's look at a common anti pattern of IoC usage:

<[sample:ShippingScreenPresenter-anti-pattern]>

Instead of binding `ShippingScreenPresenter` so tightly to StructureMap and having to explicitly fetch its dependencies, let's switch
it to using StructureMap a little more idiomatically and just exposing a constructor function with the necessary dependencies
as arguments:

<[sample:ShippingScreenPresenter-with-ctor-injection]>

As long as a StructureMap `Container` knows how to resolve the `IRepository` and
`IShippingService` interfaces, StructureMap can build `ShippingScreenPresenter` by using "auto-wiring." All this means is that
instead of forcing you to explicitly configure all the dependencies for `ShippingScreenPresenter`, StructureMap can infer from
the public <[linkto:registration/constructor-selection;title=constructor function]> and <[linkto:setter-injection;title=public property setters]>
what dependencies `ShippingScreenPresenter` needs and uses the defaults of both to build it out.

Looking at the <[linkto:diagnostics/build-plans;title=build plan]> for `ShippingScreenPresenter`:

<[sample:ShippingScreenPresenter-build-plan]>

give us:

<pre>
PluginType: StructureMap.Testing.DocumentationExamples.ShippingScreenPresenter
Lifecycle: Transient
new ShippingScreenPresenter(IShippingService, IRepository)
  ┣ IShippingService = **Default**
  ┃                     | PluginType: StructureMap.Testing.DocumentationExamples.IShippingService
  ┃                     | Lifecycle: Transient
  ┃                     | new InternalShippingService()
  ┃                    
  ┗ IRepository = **Default**
               | PluginType: StructureMap.Testing.DocumentationExamples.IRepository
               | Lifecycle: Transient
               | new SimpleRepository()
              
</pre>

See <[linkto:the-container/primitives]> for more information on how StructureMap deals with primitive types like numbers, strings, enums, and dates in auto-wiring.

