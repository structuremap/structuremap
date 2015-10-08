<!--Title: Integrating StructureMap into Common .Net Frameworks-->

Want to see more integrations or samples of how to integrate StructureMap with various .Net development tools? Let us know in Gitter or Github, or better yet, **we take pull requests for documentation.**

## ASP.Net MVC 5

The StructureMap team recommends the [StructureMap.MVC5](https://www.nuget.org/packages/StructureMap.MVC5/) nuget for integrating
StructureMap into pre-DNX ASP.Net MVC. The design is necessarily odd to work around ASP.Net limitations, but you do get the all
important "<[linkto:the-container/nested-containers;title=nested container]> per HTTP request" pattern that we recommend.

## Web API (pre-DNX)

The StructureMap team recommends the [StructureMap.WebApi2](https://www.nuget.org/packages/StructureMap.WebApi2/) nuget package. 
The IoC integration in Web API is somewhat better designed than ASP.Net MVC's bolt on mechanism and the StructureMap integration
is quite a bit cleaner.

## DNX

See the [StructureMap.DNX](https://github.com/structuremap/structuremap.dnx) project and Nuget for an early version of integration with DNX.

## NServiceBus

Use the [NServiceBus StructureMap](https://www.nuget.org/packages/NServiceBus.StructureMap/) nuget for integrating StructureMap 3 into
NServiceBus.


## NancyFx

NancyFx provides the [Nancy.BootStrappers.StructureMap](https://www.nuget.org/packages/Nancy.BootStrappers.StructureMap/) nuget package. The early 
versions of the NancyFx integration with StructureMap were deeply flawed but improved in later versions, so do make sure that you are using the latest version of the bootstrapper.

## MassTransit

Use the [MassTransit.StructureMap](http://www.nuget.org/packages/MassTransit.StructureMap/) nuget package. **Unfortunately**, this package
depends on the [signed version of StructureMap](http://www.nuget.org/packages/structuremap-signed/), so prepare yourself for version conflicts,
binding redirects, and the fusion log viewer. See the [MassTransit documentation on using StructureMap](http://docs.masstransit-project.com/en/latest/usage/containers/structuremap.html) for more information.


## Entity Framework

The best example is [this blog post from Jimmy Bogard](https://lostechies.com/jimmybogard/2013/12/20/proper-sessiondbcontext-lifecycle-management/).

