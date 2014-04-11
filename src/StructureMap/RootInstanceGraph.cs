using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    public class RootInstanceGraph : IInstanceGraph
    {
        private readonly PluginGraph _pluginGraph;


        public RootInstanceGraph(PluginGraph pluginGraph)
        {
            _pluginGraph = pluginGraph;
        }

        public Instance GetDefault(Type pluginType)
        {
            return _pluginGraph.Families[pluginType].GetDefaultInstance();
        }

        public bool HasDefaultForPluginType(Type pluginType)
        {
            return _pluginGraph.HasDefaultForPluginType(pluginType);
        }

        public bool HasInstance(Type pluginType, string instanceKey)
        {
            return _pluginGraph.HasInstance(pluginType, instanceKey);
        }

        public void EachInstance(Action<Type, Instance> action)
        {
            _pluginGraph.EachInstance(action);
        }

        public IEnumerable<Instance> GetAllInstances()
        {
            return _pluginGraph.Families.SelectMany(x => x.Instances);
        }

        public IEnumerable<Instance> GetAllInstances(Type pluginType)
        {
            return _pluginGraph.AllInstances(pluginType);
        }

        public Instance FindInstance(Type pluginType, string name)
        {
            return _pluginGraph.FindInstance(pluginType, name);
        }

        public IEnumerable<PluginFamily> UniqueFamilies()
        {
            return _pluginGraph.Families;
        }

        public ILifecycle DefaultLifecycleFor(Type pluginType)
        {
            if (pluginType == null) return null;

            if (!_pluginGraph.HasFamily(pluginType)) return null;

            return _pluginGraph.Families[pluginType].Lifecycle;
        }

        public ContainerRole Role
        {
            get { return ContainerRole.Root; }
        }

        public IEnumerable<Instance> ImmediateInstances()
        {
            return GetAllInstances();
        }
    }
}