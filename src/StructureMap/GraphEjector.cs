using System;
using System.Linq;
using StructureMap.Graph;

namespace StructureMap
{
    // Tested w/ integration tests through Container.Model
    public class GraphEjector : IGraphEjector
    {
        private readonly PluginGraph _pluginGraph;

        public GraphEjector(PluginGraph pluginGraph)
        {
            _pluginGraph = pluginGraph;
        }

        public void EjectAllInstancesOf<T>()
        {
            _pluginGraph.EjectFamily(typeof(T));
        }

        public void Remove(Func<Type, bool> filter)
        {
            _pluginGraph.Families.Where(x => filter(x.PluginType)).Select(x => x.PluginType)
                        .ToArray().Each(x => _pluginGraph.EjectFamily(x));
        }

        public void Remove(Type pluginType)
        {
            _pluginGraph.EjectFamily(pluginType);
        }
    }
}