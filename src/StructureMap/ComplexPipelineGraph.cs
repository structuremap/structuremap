using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Query;

namespace StructureMap
{
    // TODO -- introduce a base class for PipelineGraph
    public class ComplexPipelineGraph : IPipelineGraph, IInstanceGraph
    {
        private readonly IPipelineGraph _parent;
        private readonly PluginGraph _pluginGraph;
        private readonly IObjectCache _transientCache;

        public ComplexPipelineGraph(IPipelineGraph parent, PluginGraph pluginGraph, IObjectCache transientCache)
        {
            _parent = parent;
            _pluginGraph = pluginGraph;
            _transientCache = transientCache;
        }

        public IInstanceGraph Instances
        {
            get
            {
                return this;
            }
        }

        // different, but inject into ctor
        public IObjectCache Singletons
        {
            get { return _parent.Singletons; }
        }

        // Inject into ctor
        public IObjectCache Transients
        {
            get { return _transientCache; }
        }

        // Same
        public IModel ToModel()
        {
            return new Model(this, _pluginGraph);
        }

        // Same
        public string Profile
        {
            get
            {
                return _pluginGraph.ProfileName;
            }
        }

        // Same
        public IGraphEjector Ejector
        {
            get
            {
                return new GraphEjector(_pluginGraph, this);
            }
        }

        // Same
        public Policies Policies
        {
            get
            {
                return _pluginGraph.Root.Policies;
            }
        }

        // Same
        public void Dispose()
        {
            _transientCache.DisposeAndClear();
            _pluginGraph.SafeDispose();
        }

        // Identical to RootPipelineGraph
        public IPipelineGraph ToNestedGraph()
        {
            return new ComplexPipelineGraph(this, new PluginGraph("Nested"), new NestedContainerTransientObjectCache());
        }

        // Same
        public void RegisterContainer(IContainer container)
        {
            _pluginGraph.Families[typeof(IContainer)].SetDefault(new ObjectInstance(container));
        }

        // Same
        public void Configure(Action<ConfigurationExpression> configure)
        {
            var registry = new ConfigurationExpression();
            configure(registry);

            var builder = new PluginGraphBuilder(_pluginGraph);
            builder.Add(registry);

            builder.RunConfigurations();
        }



        /******************************************************************/
        // Different -- do with Func
        public IPipelineGraph Root()
        {
            return _parent.Root();
        }

        // Different
        public Instance GetDefault(Type pluginType)
        {
            return _pluginGraph.HasDefaultForPluginType(pluginType)
                ? _pluginGraph.Families[pluginType].GetDefaultInstance()
                : _parent.Instances.GetDefault(pluginType);
        }

        // Different
        public bool HasDefaultForPluginType(Type pluginType)
        {
            return _pluginGraph.HasDefaultForPluginType(pluginType) || _parent.Instances.HasDefaultForPluginType(pluginType);
        }

        // Different
        public bool HasInstance(Type pluginType, string instanceKey)
        {
            return _pluginGraph.HasInstance(pluginType, instanceKey) || _parent.Instances.HasInstance(pluginType, instanceKey);
        }

        // Different
        public void EachInstance(Action<Type, Instance> action)
        {
            _pluginGraph.EachInstance(action);
            _parent.Instances.EachInstance(action);
        }

        // Different
        public IEnumerable<Instance> GetAllInstances()
        {
            return _pluginGraph.Families.SelectMany(x => x.Instances).Union(_parent.Instances.GetAllInstances());
        }

        // Different
        public IEnumerable<Instance> GetAllInstances(Type pluginType)
        {
            return _pluginGraph.AllInstances(pluginType).Union(_parent.Instances.GetAllInstances(pluginType));
        }

        // Different
        public Instance FindInstance(Type pluginType, string name)
        {
            return _pluginGraph.FindInstance(pluginType, name) ?? _parent.Instances.FindInstance(pluginType, name);
        }

        // Different -- but do w/ Func
        public IPipelineGraph ForProfile(string profile)
        {
            return _parent.ForProfile(profile);
        }


        // Different
        public IEnumerable<PluginFamily> UniqueFamilies()
        {
            foreach (var family in _pluginGraph.Families)
            {
                yield return family;
            }

            foreach (var family in _parent.Instances.UniqueFamilies().Where(x => !_pluginGraph.Families.Has(x.PluginType)))
            {
                yield return family;
            }
        }
    }
}