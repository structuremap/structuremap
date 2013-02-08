using StructureMap.Graph;

namespace StructureMap.Configuration
{
    public interface IPluginGraphConfiguration
    {
        void Configure(PluginGraph graph);
        void Register(PluginGraphBuilder builder);
    }

}