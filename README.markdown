Welcome to StructureMap, the oldest [Inversion of Control][1] container. 

Getting StructureMap
--------------------

StructureMap is available via NuGet:

    > Install-Package StructureMap


Also, you can get it from the downloads area. If you want to fix a bug or just want to tinker with an idea,
we love receiving pull requests!

Building the Source
-------------------

 1. Clone the repository: `git clone git://github.com/structuremap/structuremap.git`
 1. make sure that you have got [ruby installed][2].
 1. run `gem install albacore`
 1. Necessary files for the build are located in the __buildsupport__ git module. 
    To obtain the contents run `git submodule update --init`.
    The __--init__ flag is only necessary the first time you run it. 
 1. In the root, run `rake`.
 1. Open `StructureMap.sln` in VS2010.
 1. Build

Debugger Visualizers
--------------------

These were an experiment to find a better way to analyze problems in the container. To enable the debugger
visualizers in Visual Studio 2008, put a copy of `StructureMap.dll` and
`StructureMap.DebuggerVisualizers.dll` in `<My Documents>\Visual Studio 2008\Visualizers`

**WARNING**: The visualizer is very early and not well tested. You may experience issues (unhandled
exceptions while using the visualizer) if the version of StructureMap.dll in your project is not the
exact version in your Visualizers folder.

A copy of the StructureMap website and documentation is in the "Docs" folder.

Please post any questions or bugs to the StructureMap Users mailing list:
http://groups.google.com/group/structuremap-users

The latest code and documentation is available on the main site:
http://structuremap.net/

Thanks for trying StructureMap.

 [1]: http://docs.structuremap.net/InversionOfControl.htm
 [2]: http://www.ruby-lang.org/en/downloads/