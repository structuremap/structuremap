Welcome to StructureMap, the oldest Inversion of Control container for .Net. 

Getting StructureMap
--------------------

[![Join the chat at https://gitter.im/structuremap/structuremap](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/structuremap/structuremap?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

StructureMap is available via NuGet:

```PowerShell
Install-Package StructureMap
```


If you want to fix a bug or just want to tinker with an idea,
we love receiving pull requests!

Building the Source
-------------------

1. Clone the repository: `git clone git://github.com/structuremap/structuremap.git`
1. Open the solution at src/StructureMap.sln and go to town! Note that Paket is used for auto-restoring Nuget dependencies as part of the MSBuild compilation. Just compile through VS.Net to retrieve all the dependencies.

Note:

The StructureMap team uses Rake internally and on the CI server, but **Rake is no longer necessary in any way for developing with the StructureMap codebase**.

Please post any questions or bugs to the
[StructureMap Users mailing list](https://groups.google.com/forum/#!forum/structuremap-users).

The latest documentation is available at [http://structuremap.github.io](http://structuremap.github.io).

Thanks for trying StructureMap.
