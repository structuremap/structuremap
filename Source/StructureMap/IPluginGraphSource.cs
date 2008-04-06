using StructureMap.Graph;

namespace StructureMap
{
    public interface IPluginGraphSource
    {
        InstanceDefaultManager DefaultManager { get; }

        /// <summary>
        /// Reads the configuration information and returns the PluginGraph definition of
        /// plugin families and plugin's
        /// </summary>
        /// <returns></returns>
        PluginGraph Build();
    }
}