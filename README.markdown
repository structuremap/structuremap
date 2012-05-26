Welcome to StructureMap, the oldest [Inversion of Control][1] container. 

Getting StructureMap
--------------------

StructureMap is available via NuGet:

```powershell
> Install-Package StructureMap
```

Also, you can get it from the downloads area. If you want to fix a bug or just want to tinker with an idea,
we love receiving pull requests!

Building the Source
-------------------

To start using StructureMap, either use the DLL's in the "deploy" folder or click on the RunBuild.BAT
file to run the full NAnt build.  There is a known issue with the build "sticking" on the custom NAnt
tasks.  If this happens, delete the copies of StructureMap.Dll and StructureMap.DeploymentTasks.Dll
in the bin\NAnt folder.  Look in the "build" directory for the build products.

 1. Clone the repository: `git clone git://github.com/structuremap/structuremap.git`
 2. Run `nant` from the command line (if you have nant installed globally, you literally run `nant`).
 This generates the appropriate `CommonAssemblyInfo.cs` file that is linked into every project.
 3. Open `StructureMap.sln` in Visual Studio 2008 or `StructureMap_2010.sln` in VS2010.
 4. Build

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
