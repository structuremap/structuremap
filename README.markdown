To start using StructureMap, either use the DLL's in the "deploy" folder or click on the RunBuild.BAT file to run the full NAnt build.  There is a known issue with the build "sticking" on the custom NAnt tasks.  If this happens, delete the copies of StructureMap.Dll and StructureMap.DeploymentTasks.Dll in the bin\NAnt folder.  Look in the "build" directory for the build products.

To enable the debugger visualizers in Visual Studio 2008, put a copy of StructureMap.dll and StructureMap.DebuggerVisualizers.dll in "<My Documents>\Visual Studio 2008\Visualizers"
**WARNING: The visualizer is very early and not well tested. You may experience issues (unhandled exceptions while using the visualizer) if the version of StructureMap.dll in your project is not the exact version in your Visualizers folder.

A copy of the StructureMap website and documentation is in the "Docs" folder.


Please post any questions or bugs to the StructureMap Users mailing list:
http://groups.google.com/group/structuremap-users


The latest code and documentation is available on SourceForge:
http://structuremap.sourceforge.net/

Thanks for trying StructureMap.
