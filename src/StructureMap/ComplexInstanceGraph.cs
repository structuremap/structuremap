using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    public class ComplexInstanceGraph : IInstanceGraph
    {
        private readonly IPipelineGraph _parent;
        private readonly PluginGraph _pluginGraph;

        public ComplexInstanceGraph(IPipelineGraph parent, PluginGraph pluginGraph, ContainerRole role)
        {
            _parent = parent;
            _pluginGraph = pluginGraph;
            Role = role;
        }

        public Instance GetDefault(Type pluginType)
        {
            return _pluginGraph.HasDefaultForPluginType(pluginType)
                ? _pluginGraph.Families[pluginType].GetDefaultInstance()
                : _parent.Instances.GetDefault(pluginType);
        }

        public bool HasDefaultForPluginType(Type pluginType)
        {
            return _pluginGraph.HasDefaultForPluginType(pluginType) ||
                   _parent.Instances.HasDefaultForPluginType(pluginType);
        }

        public bool HasInstance(Type pluginType, string instanceKey)
        {
            return _pluginGraph.HasInstance(pluginType, instanceKey) ||
                   _parent.Instances.HasInstance(pluginType, instanceKey);
        }

        public void EachInstance(Action<Type, Instance> action)
        {
            _pluginGraph.EachInstance(action);
            _parent.Instances.EachInstance(action);
        }

        public IEnumerable<Instance> GetAllInstances()
        {
            return _pluginGraph.Families.SelectMany(x => x.Instances).Union(_parent.Instances.GetAllInstances());
        }

        public IEnumerable<Instance> GetAllInstances(Type pluginType)
        {
            return _pluginGraph.AllInstances(pluginType).Union(_parent.Instances.GetAllInstances(pluginType));
        }

        public Instance FindInstance(Type pluginType, string name)
        {
            return _pluginGraph.FindInstance(pluginType, name) ?? _parent.Instances.FindInstance(pluginType, name);
        }

        public IEnumerable<PluginFamily> UniqueFamilies()
        {
            foreach (var family in _pluginGraph.Families)
            {
                yield return family;
            }

            foreach (
                var family in _parent.Instances.UniqueFamilies().Where(x => !_pluginGraph.Families.Has(x.PluginType)))
            {
                yield return family;
            }
        }

        public ILifecycle DefaultLifecycleFor(Type pluginType)
        {
            if (_pluginGraph.Families.Has(pluginType))
            {
                var family = _pluginGraph.Families[pluginType];
                if (family.Lifecycle != null) return family.Lifecycle;
            }

            return _parent.Instances.DefaultLifecycleFor(pluginType);
        }

        public ContainerRole Role { get; private set; }
        public IEnumerable<Instance> ImmediateInstances()
        {
            return _pluginGraph.Families.SelectMany(x => x.Instances);
        }

        public PluginGraph ImmediatePluginGraph
        {
            get
            {
                return _pluginGraph;
            }
        }
    }
}