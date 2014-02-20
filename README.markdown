Welcome to StructureMap, the oldest [Inversion of Control][1] container for .Net. 

Getting StructureMap
--------------------

StructureMap is available via NuGet:

    > Install-Package StructureMap

       -- or --

    > ripple install StructureMap -p [your project name]

If you want to fix a bug or just want to tinker with an idea,
we love receiving pull requests!

Building the Source
-------------------

 1. Clone the repository: `git clone git://github.com/structuremap/structuremap.git`
 1. Make sure that you have got [ruby installed][3] (>= 1.9.3).
 1. If you are installing Ruby for the first time, install Bundler with "gem install bundler" at a command prompt
 1. In the root, run `rake`.
 1. Open `StructureMap.sln` in VS2012 ('rake sln' from the root as well).




A copy of the StructureMap website and documentation is in the "StructureMap.Docs" folder.  To run the documentation website, run 'rake fubudocs:run' or 'fubudocs run -o' in the command prompt

Please post any questions or bugs to the StructureMap Users mailing list:
http://groups.google.com/group/structuremap-users

The latest code and documentation is available on the main site:
http://structuremap.net/ and http://fubuworld.com/structuremap

Thanks for trying StructureMap.

 [1]: http://docs.structuremap.net/InversionOfControl.htm
 [3]: http://www.ruby-lang.org/en/downloads/
 
