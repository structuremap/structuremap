using System;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StructureMap.Configuration
{
    [Obsolete("I think this thing can go away and just get folded into InstanceParser")]
    public class GraphBuilder : IGraphBuilder
    {
        private readonly PluginGraph _pluginGraph;


        public GraphBuilder(PluginGraph pluginGraph)
        {
            _pluginGraph = pluginGraph;
        }

        public PluginGraph PluginGraph
        {
            get { return _pluginGraph; }
        }

        public void AddRegistry(string registryTypeName)
        {
            _pluginGraph.Log.Try(() => {
                var type = new TypePath(registryTypeName).FindType();
                var registry = (Registry) Activator.CreateInstance(type);
                registry.As<IPluginGraphConfiguration>().Configure(_pluginGraph);
            }).AndReportErrorAs(290, registryTypeName);
        }

        public void ConfigureFamily(TypePath pluginTypePath, Action<PluginFamily> action)
        {
            try
            {
                var pluginType = pluginTypePath.FindType();
                var family = _pluginGraph.Families[pluginType];
                action(family);
            }
            catch (Exception ex)
            {
                _pluginGraph.Log.RegisterError(ex);
            }
        }
    }
}