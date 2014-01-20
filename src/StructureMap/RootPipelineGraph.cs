using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Query;
using StructureMap.Util;

namespace StructureMap
{
    public class Profiles
    {
        private readonly Cache<string, IPipelineGraph> _profiles;

        public Profiles(PluginGraph pluginGraph, IPipelineGraph root)
        {
            _profiles =
                new Cache<string, IPipelineGraph>(
                    name => new ComplexPipelineGraph(root, pluginGraph.Profile(name), new NulloTransientCache()));
        }

        public IPipelineGraph For(string profileName)
        {
            return _profiles[profileName];
        }
    }

    public class RootPipelineGraph : IPipelineGraph, IInstanceGraph
    {
        private readonly PluginGraph _pluginGraph;

        private readonly IObjectCache _transientCache;
        private readonly Profiles _profiles;

        public RootPipelineGraph(PluginGraph pluginGraph)
        {
            _pluginGraph = pluginGraph;
            _transientCache = new NulloTransientCache();
            _profiles = new Profiles(_pluginGraph, this);
        }

        // Same
        public Profiles Profiles
        {
            get { return _profiles; }
        }

        public IInstanceGraph Instances
        {
            get { return this; }
        }

        public IObjectCache Singletons
        {
            get { return _pluginGraph.SingletonCache; }
        }

        public IObjectCache Transients
        {
            get { return _transientCache; }
        }

        public IModel ToModel()
        {
            return new Model(this, _pluginGraph);
        }

        public string Profile
        {
            get { return _pluginGraph.ProfileName; }
        }

        public IGraphEjector Ejector
        {
            get { return new GraphEjector(_pluginGraph, this); }
        }

        public Policies Policies
        {
            get { return _pluginGraph.Root.Policies; }
        }

        public IPipelineGraph Root()
        {
            return this;
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

        public void Dispose()
        {
            _pluginGraph.SafeDispose();
            _transientCache.DisposeAndClear();
        }

        public IPipelineGraph ToNestedGraph()
        {
            return new ComplexPipelineGraph(this, new PluginGraph("Nested"), new NestedContainerTransientObjectCache());
        }

        public IEnumerable<PluginFamily> UniqueFamilies()
        {
            return _pluginGraph.Families;
        }

        public void RegisterContainer(IContainer container)
        {
            _pluginGraph.Families[typeof (IContainer)].SetDefault(new ObjectInstance(container));
        }

        public void Configure(Action<ConfigurationExpression> configure)
        {
            var registry = new ConfigurationExpression();
            configure(registry);

            var builder = new PluginGraphBuilder(_pluginGraph);
            builder.Add(registry);

            builder.RunConfigurations();
        }

        public static RootPipelineGraph For(Action<ConfigurationExpression> action)
        {
            var expression = new ConfigurationExpression();
            action(expression);

            var graph = expression.BuildGraph();

            return new RootPipelineGraph(graph);
        }
    }
}