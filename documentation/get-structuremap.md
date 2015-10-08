<!--Title: Get StructureMap-->
<!--Url: get-structuremap-->


## Current Release

The current version of StructureMap will always be available on [Nuget](https://www.nuget.org/packages/structuremap/). See the <[linkto:release-notes]> for more information.



## Supported Platforms

The original `StructureMap.dll` has been broken up into a couple pieces. The main assembly will be targeting PCL compliance thanks to the diligent efforts of [Frank Quednau](https://twitter.com/fquednau), and that means that anything to do with ASP.Net or falls outside of the PCL core has been devolved into separate assemblies and eventually into different Nuget packages. This means that StructureMap 3.* will *theoretically* support WP8 and other versions of .Net for the very first time.

At this point StructureMap 3.* has been used on .Net 4, 4.5, WP8, and WP8.1. Nobody in the core StructureMap team is currently working with Xamarin mobile runtimes, but we are interested in verifying StructureMap on new platforms if any volunteers are interested in helping us out.

## Binaries

Binaries of StructureMap are available via [NuGet](http://www.nuget.org/packages/structuremap/):

    PM> Install-Package StructureMap

or if you need a strong named version of StructureMap:

    PM> Install-Package structuremap-signed

**The StructureMap team believes strong naming to be a blight upon the .Net land and we really wish folks would quit using it, but we understand that some circumstances absolutely require it.**

## Source

StructureMap's source is hosted on [GitHub](https://github.com/structuremap/structuremap/). If you want to fix a bug or just want to tinker with an idea, we love receiving pull requests! Start by creating your own [fork](https://github.com/structuremap/structuremap/fork) on Github.

The StructureMap build tooling has recently changed. See the [GitHub README](https://github.com/structuremap/structuremap/blob/master/README.markdown) for more information on how to build and work with the StructureMap codebase.

