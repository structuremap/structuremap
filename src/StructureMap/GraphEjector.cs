using System;
using System.Linq;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    // Tested w/ integration tests through Container.Model
    public class GraphEjector : IGraphEjector
    {
        private readonly PluginGraph _pluginGraph;
        private readonly ILifecycleContext _lifecycles;

        public GraphEjector(PluginGraph pluginGraph, ILifecycleContext lifecycles)
        {
            _pluginGraph = pluginGraph;
            _lifecycles = lifecycles;
        }

        public void EjectAllInstancesOf<T>()
        {
            _pluginGraph.EjectFamily(typeof (T));
        }

        public void RemoveCompletely(Func<Type, bool> filter)
        {
            _pluginGraph.Families.Where(x => filter(x.PluginType))
                .Select(x => x.PluginType)
                .ToArray()
                .Each(RemoveCompletely);
        }

        public void RemoveCompletely(Type pluginType)
        {
            if (!_pluginGraph.Families.Has(pluginType)) return;

            var family = _pluginGraph.Families[pluginType];
            family.Instances.Each(i => RemoveFromLifecycle(pluginType, i));

            _pluginGraph.EjectFamily(pluginType);
        }

        public void RemoveFromLifecycle(Type pluginType, Instance instance)
        {
            instance.Lifecycle.FindCache(_lifecycles).Eject(pluginType, instance);
        }

        public void RemoveCompletely(Type pluginType, Instance instance)
        {
            RemoveFromLifecycle(pluginType, instance);

            _pluginGraph.Families[pluginType].RemoveInstance(instance);

            instance.SafeDispose();

        }
    }
}