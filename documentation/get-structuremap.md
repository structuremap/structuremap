<!--Title: Get StructureMap-->
<!--Url: get-structuremap-->


## Current Release

The current version of StructureMap will always be available on [Nuget](https://www.nuget.org/packages/structuremap/). See the <[linkto:release-notes]> for more information.



## Supported Platforms

The original `StructureMap.dll` has been broken up into a couple pieces. The main assembly will be targeting PCL compliance thanks to the diligent efforts of [Frank Quednau](https://twitter.com/fquednau), and that means that anything to do with ASP.Net or falls outside of the PCL core has been devolved into separate assemblies and eventually into different Nuget packages. This means that StructureMap 3.* will *theoretically* support WP8 and other versions of .Net for the very first time.

At this point StructureMap 3.* has been used on .Net 4, 4.5, and WP8. Nobody in the core StructureMap team is currently working with Xamarin mobile runtimes, but we are interested in verifying StructureMap on new platforms if any volunteers are interested.

## Binaries

Binaries of StructureMap are available via [NuGet](http://www.nuget.org/packages/structuremap/):

    PM> Install-Package StructureMap

## Source

StructureMap's source is hosted on [GitHub](https://github.com/structuremap/structuremap/). If you want to fix a bug or just want to tinker with an idea, we love receiving pull requests! Start by creating your own [fork](https://github.com/structuremap/structuremap/fork) on Github.


<Section title="Building the Source" id="building-source">

StructureMap needs [FubuRake][2] and [Ripple][1] for building the source. FubuRake is an OSS Ruby library to quickly stand up a fully functional, cross platform [`rake`][3] script for a .Net codebase based on Fubu project standards. Ripple is a package manager that simplifies managing Nuget dependencies in your solution. Both dependencies are currently distributed through Ruby Gems and are part of the Fubu family. Before you continue you need to make sure that you have got Ruby installed.


1. Clone the repository: `git clone git://github.com/structuremap/structuremap.git`
1. Now really make sure that you have got Ruby 1.9+ installed
1. See the README in the source code for instructions on using gems to install rake and other gems
1. In the root of the StructureMap Git repository, run `rake`. 
1. Open StructureMap.sln in VS2012 ('rake sln' from the root as well).
1. Build.

[1]: http://fubuworld.com/ripple/
[2]: http://fubuworld.com/fuburake/
[3]: https://github.com/jimweirich/rake



<br/>

> **Installing Ruby, Rake and RubyGems**

> Installing Ruby will give you RubyGems and Rake out of the box. You can find installation guidance at the official [Ruby language](https://www.ruby-lang.org/en/installation/) website or you could install Ruby via [Chocolatey](http://chocolatey.org/):
 
> `> cinst ruby`

> After you have Ruby, Rake and RubyGems installed you are good to go. The `rake` build file `rakefile.rb` (located in the root) will install al the other needed dependencies like `FubuRake` and `Ripple`, which are defined in the `Gemfile` (also located in the root). This file is used by another gem called [`bundler`](http://bundler.io/). The `rake` build file will automatically install `bundler`, which does the same for all the dependencies listed in the `Gemfile`, but only if they weren't already installed on your machine.

