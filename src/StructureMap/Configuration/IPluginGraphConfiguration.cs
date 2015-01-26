using StructureMap.Graph;

namespace StructureMap.Configuration
{
    /// <summary>
    /// Interacts with PluginGraph
    /// </summary>
    public interface IPluginGraphConfiguration
    {
        /// <summary>
        /// Configure an already built <see cref="PluginGraph"/>
        /// </summary>
        void Configure(PluginGraph graph);

        /// <summary>
        /// Registers an PluginGraphBuilder
        /// </summary>
        /// <param name="builder"></param>
        void Register(PluginGraphBuilder builder);
    }
}