<!--Title: Integrating StructureMap into Common .Net Frameworks-->

Want to see more integrations or samples of how to integrate StructureMap with various .Net development tools? Let us know in Gitter or Github, or better yet, **we take pull requests for documentation.**

## ASP.NET MVC 5

The StructureMap team recommends the [StructureMap.MVC5](https://www.nuget.org/packages/StructureMap.MVC5/) NuGet package for integrating
StructureMap into ASP.NET MVC. The design is necessarily odd to work around ASP.NET limitations, but you do get the all
important "<[linkto:the-container/nested-containers;title=nested container]> per HTTP request" pattern that we recommend.

## ASP.NET Web API

The easiest way to integrate StructureMap with ASP.NET Web API is with the [WebApi.StructureMap](https://www.nuget.org/packages/WebApi.StructureMap) NuGet package. This package requires StructureMap 4.1 or higher.
Use the [StructureMap.WebApi2](https://www.nuget.org/packages/StructureMap.WebApi2/) NuGet package for older versions of StructureMap or combined ASP.NET MVC and Web API projects where you already use the [StructureMap.MVC5](https://www.nuget.org/packages/StructureMap.MVC5/) NuGet package.

## ASP.NET Core

See the [StructureMap.Microsoft.DependencyInjection](https://github.com/structuremap/StructureMap.Microsoft.DependencyInjection) project and NuGet package for integration with ASP.NET Core.

## NServiceBus

Use the [NServiceBus StructureMap](https://www.nuget.org/packages/NServiceBus.StructureMap/) NuGet package for integrating StructureMap 3 into
NServiceBus.

## NancyFx

NancyFx provides the [Nancy.Bootstrappers.StructureMap](https://www.nuget.org/packages/Nancy.Bootstrappers.StructureMap/) NuGet package. The early 
versions of the NancyFx integration with StructureMap were deeply flawed but improved in later versions, so do make sure that you are using the latest version of the bootstrapper.

## MassTransit

Use the [MassTransit.StructureMap](http://www.nuget.org/packages/MassTransit.StructureMap/) NuGet package. **Unfortunately**, this package
depends on the [signed version of StructureMap](http://www.nuget.org/packages/structuremap-signed/), so prepare yourself for version conflicts,
binding redirects, and the fusion log viewer. See the [MassTransit documentation on using StructureMap](http://docs.masstransit-project.com/en/latest/usage/containers/structuremap.html) for more information.

## Entity Framework

The best example is [this blog post from Jimmy Bogard](https://lostechies.com/jimmybogard/2013/12/20/proper-sessiondbcontext-lifecycle-management/).

## AutoMapper

See [My AutoMapper setup for StructureMap](http://www.martijnburgers.net/post/2013/12/20/My-AutoMapper-setup-for-StructureMap.aspx) from Martijn Burgers.
