using StructureMap.Configuration;
using StructureMap.Graph;

namespace StructureMap
{
    public interface IPluginGraphSource
    {
        InstanceDefaultManager DefaultManager { get; }

        PluginGraphReport Report { get; }

        /// <summary>
        /// Reads the configuration information and returns the PluginGraph definition of
        /// plugin families and plugin's
        /// </summary>
        /// <returns></returns>
        PluginGraph Build();

        /// <summary>
        /// Build a PluginGraph with all instances calculated.  Used in the UI and diagnostic tools.
        /// </summary>
        /// <returns></returns>
        PluginGraph BuildDiagnosticPluginGraph();
    }
}