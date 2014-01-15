using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    public class ComplexPipelineGraph : IPipelineGraph
    {
        private readonly IPipelineGraph _parent;
        private readonly PluginGraph _outer;
        private readonly IObjectCache _transientCache;

        public ComplexPipelineGraph(IPipelineGraph parent, PluginGraph outer, IObjectCache transientCache)
        {
            _parent = parent;
            _outer = outer;
            _transientCache = transientCache;
        }

        public IObjectCache Singletons
        {
            get { return _parent.Singletons; }
        }

        public IObjectCache Transients
        {
            get { return _transientCache; }
        }

        public IPipelineGraph Root()
        {
            return _parent.Root();
        }

        public Instance GetDefault(Type pluginType)
        {
            return _outer.HasDefaultForPluginType(pluginType)
                ? _outer.Families[pluginType].GetDefaultInstance()
                : _parent.GetDefault(pluginType);
        }

        public bool HasDefaultForPluginType(Type pluginType)
        {
            return _outer.HasDefaultForPluginType(pluginType) || _parent.HasDefaultForPluginType(pluginType);
        }

        public bool HasInstance(Type pluginType, string instanceKey)
        {
            return _outer.HasInstance(pluginType, instanceKey) || _parent.HasInstance(pluginType, instanceKey);
        }

        public void EachInstance(Action<Type, Instance> action)
        {
            _outer.EachInstance(action);
            _parent.EachInstance(action);
        }

        public IEnumerable<Instance> GetAllInstances()
        {
            return _outer.Families.SelectMany(x => x.Instances).Union(_parent.GetAllInstances());
        }

        public IEnumerable<Instance> GetAllInstances(Type pluginType)
        {
            return _outer.AllInstances(pluginType).Union(_parent.GetAllInstances(pluginType));
        }

        public Instance FindInstance(Type pluginType, string name)
        {
            return _outer.FindInstance(pluginType, name) ?? _parent.FindInstance(pluginType, name);
        }

        public IPipelineGraph ForProfile(string profile)
        {
            return _parent.ForProfile(profile);
        }


        public void Dispose()
        {
            _transientCache.DisposeAndClear();
            _outer.SafeDispose();
        }

        // Identical to RootPipelineGraph
        public IPipelineGraph ToNestedGraph()
        {
            return new ComplexPipelineGraph(this, new PluginGraph("Nested"), new NestedContainerTransientObjectCache());
        }

        public IEnumerable<PluginGraph> AllGraphs()
        {
            foreach (var pluginGraph in _parent.AllGraphs())
            {
                yield return pluginGraph;
            }

            yield return _outer;
        }

        public PluginGraph Outer
        {
            get { return _outer; }
        }

        public IEnumerable<PluginFamily> UniqueFamilies()
        {
            foreach (var family in _outer.Families)
            {
                yield return family;
            }

            foreach (var family in _parent.UniqueFamilies().Where(x => !_outer.Families.Has(x.PluginType)))
            {
                yield return family;
            }
        }
    }
}