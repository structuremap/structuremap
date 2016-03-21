<!--Title: Release Notes-->
<!--Url: release-notes-->

StructureMap is attempting to follow a strict [SemVer](http://semver.org) versioning policy.

## Release Notes 4.1

4.1 was mostly a bugfix release, but also included some new public API calls for type scanning discovery from *.exe files.
See the [closed GitHub issues](https://github.com/structuremap/structuremap/issues?q=is%3Aissue+milestone%3A4.1+is%3Aclosed) for details.

* 4.1.2 - Bug fix for [GH-461](https://github.com/structuremap/structuremap/issues/461) singleton scoped registrations to a child container
* 4.1.1 - Bug fix for "AlwaysUnique" lifecycle within enumerable, inline dependencies


## Release Notes 4.0

4.0.1 was strictly a bug fix release. 
## Release Notes 4.0

4.0 is a medium sized release that largely builds on the existing 3.0 architecture with significant improvements to conventional
registration via type scanning and several performance improvements as well as some bug fixes.

The highlights:

1. We **believe** that the documentation is finally comprehensive in regards to StructureMap behavior and features
1. StructureMap 4.0 is cross-compiled to .Net 4.0, .Net 4.0 as/ PCL, and **as CoreCLR compliant** (using the dotnet build profile).
1. The type scanning model (`Registry.Scan()`) was completely rebuilt to make the model easier to extend and to
   *optimize application bootstrapping* in systems that heavily rely on StructureMap type scanning. See <[linkto:registration/auto-registration-and-conventions]> for more information.
1. New diagnostic method specifically for troubleshooting type scanning problems. See <[linkto:diagnostics/type-scanning]> for more information.
1. New concept of a "build time Instance convention" to apply much more powerful policies and conventions against your StructureMap registrations
   as a whole. See <[linkto:registration/policies]> for more information.
1. Runtime performance improvements
1. Hardened StructureMap for multi-threaded usage
1. Performance improvements specifically targeted at usage from within ASP.Net MVC and its particular manner of using IoC tools.
1. Removed the static `ObjectFactory` facade over the application `Container` altogether.
1. Enhanced lifecycle support for compliance with ASP.Net MVC6. Nested containers track and dispose _AlwaysUnique_ objects, the new _ContainerScoped_
   lifecyle, and an optional mode to make the root or child containers track _Transient_ disposable objects. See <[linkto:object-lifecycle]> and <[linkto:the-container/nested-containers]> for more information.
1. `Registry.For<T>().ClearAll()` and `Registry.For(Type).ClearAll()` methods for removing all previous registrations for a type. See 
   <[linkto:registration/clear-or-replace]> for more information.

See also [the complete list of changes and issues](https://github.com/structuremap/structuremap/issues?q=milestone%3A4.0+is%3Aclosed) for StructureMap 4.0.

## 3.1.*

Click on any release version to see the list of closed GitHub issues related to any of the 3.1 releases.

* [3.1.6](https://github.com/structuremap/structuremap/pulls?q=is%3Apr+milestone%3A3.1.6+is%3Aclosed) -- Optional strong named nugets, bug fixes
* [3.1.5](https://github.com/structuremap/structuremap/issues?q=is%3Aissue+milestone%3A3.1.5+is%3Aclosed) -- Fixes for open generics convention resolution, child containers and policies, naming conflicts between setters and ctor arguments
* [3.1.4](https://github.com/structuremap/structuremap/issues?q=is%3Aissue+milestone%3A3.1.4+is%3Aclosed) -- performance optimization for resolving objects that are registered directly to a Container
* [3.1.3](https://github.com/structuremap/structuremap/issues?q=is%3Aissue+milestone%3A3.1.3+is%3Aclosed) -- named instances registered to child or nested containers
* [3.1.2](https://github.com/structuremap/structuremap/issues?q=is%3Aissue+milestone%3A3.1.2+is%3Aclosed) -- reduces thread contention issues on Container.Configure() calls at runtime
* [3.1.1](https://github.com/structuremap/structuremap/issues?q=is%3Aissue+milestone%3A3.1.1+is%3Aclosed) -- fixed a potentially significant memory  leak issue
* [3.1.0](https://github.com/structuremap/structuremap/issues?q=is%3Aissue+milestone%3A3.1.0+is%3Aclosed) -- new functionality on `IContext`


## 3.0.* Bug Fix Releases

Click on any release version to see the list of closed GitHub issues related to a bugfix release.

* [3.0.5](https://github.com/structuremap/structuremap/issues?q=is%3Aissue+milestone%3A3.0.5+is%3Aclosed)
* [3.0.4](https://github.com/structuremap/structuremap/issues?q=is%3Aissue+milestone%3A3.0.4+is%3Aclosed)
* [3.0.3](https://github.com/structuremap/structuremap/issues?q=is%3Aissue+milestone%3A3.0.3+is%3Aclosed)
* [3.0.2](https://github.com/structuremap/structuremap/issues?q=is%3Aissue+milestone%3A3.0.2+is%3Aclosed)
* [3.0.1](https://github.com/structuremap/structuremap/issues?q=is%3Aissue+milestone%3A3.0.1+is%3Aclosed)

## Release Notes 3.0

- The exception messages provide contextual information about what StructureMap was trying to do when things went wrong.

- The nested container implementation is vastly improved, much faster (100X in my testing against a big application), and doesn’t have the massive singleton behavior bug from 2.6.*.

- All old `[Obsolete]` 2.5 registration syntax has been removed, and there’s been a major effort to enforce consistency throughout the registration API’s.

- The original StructureMap.dll has been broken up into a couple pieces.  The main assembly will be targeting PCL compliance thanks to the diligent efforts of Frank Quednau, and that means that Xml configuration and anything to do with ASP.Net has been devolved into separate assemblies and eventually into different Nuget packages.  This means that StructureMap will theoretically support WP8 and other versions of .Net for the very first time.  God help me.

- The strong naming has been removed.  My thought is to distribute separate Nuget packages with unsigned versions for sane folks and signed versions for enterprise-y folks.

- Lifecycle (scope) can be set individually on each Instance (stupid limitation left over from the very early days)
Constructor selection can be specified per Instance.

- Improved diagnostics, both at runtime and for the container configuration.

- Improved runtime performance, especially for deep object graphs with inline dependencies (i.e., <ProjectLink name="FubuMVC"/> behavior chains).

- The interception model has been completely redesigned.

- The ancient attribute model for StructureMap configuration has been mostly removed.

- The “Profile” model has been much improved.

- The Xml configuration has been heavily streamlined.

- Internally, the old `PipelineGraph`, `InstanceFactory`, `ProfileManager` architecture is all gone. The new `PipelineGraph` implementations just wrap one or more `PluginGraph` objects, so there’s vastly less data structure shuffling gone on internally.

**Related links:**

- http://jeremydmiller.com/2014/02/18/structuremap-3-is-gonna-tell-you-whats-wrong-and-where-it-hurts/
- http://jeremydmiller.com/2014/01/17/structuremap-3-0-is-very-nearly-done-no-seriously/
- http://jeremydmiller.com/2013/02/22/big-update-on-structuremap-3-0-progress/
- http://jeremydmiller.com/2013/01/30/lets-try-this-again-structuremap-3-0-in-en-route-as-of-now/

