<!--Title: Roadmap-->
<!--Url: roadmap-->

StructureMap 3.0 released in early 2014 marked a huge change in the StructureMap internals and public API's. 

The ongoing work for version 4.0 is a much smaller release that primarily improves type scanning,
diagnostics, and performance. The only breaking changes from 3 to 4 are in custom type scanning conventions, the removal of `ObjectFactory`, and
the elimination of some obscure configuration API's.

Please log any suspected bugs or feature requests on [StructureMap's GitHub page](https://github.com/structuremap/structuremap).


## Ongoing and upcoming work

1. The 4.0 work can be browsed from the [GitHub issue list](https://github.com/structuremap/structuremap/milestones/4.0).
1. A new package called _StructureMap.DynamicInterception_ for adding support for AOP with Castle.Core dynamic proxies to StructureMap
1. The StructureMap.AutoFactory library is being rewritten based on Castle Dynamic Proxy to add more robust capabilities similar to what Windsor has
1. The [StructureMap.DNX](https://github.com/structuremap/structuremap.dnx) package for DNX integration (an early version is already available on Nuget.org)

