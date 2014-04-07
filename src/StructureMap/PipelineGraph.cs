using System;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Query;

namespace StructureMap
{
    public class PipelineGraph : IPipelineGraph
    {
        public static IPipelineGraph For(Action<ConfigurationExpression> action)
        {
            var expression = new ConfigurationExpression();
            action(expression);

            var graph = expression.BuildGraph();

            return BuildRoot(graph);
        }

        public static IPipelineGraph BuildRoot(PluginGraph pluginGraph)
        {
            return new PipelineGraph(pluginGraph, new RootInstanceGraph(pluginGraph), null, pluginGraph.SingletonCache,
                new NulloTransientCache());
        }

        public static IPipelineGraph BuildEmpty()
        {
            var pluginGraph = new PluginGraph();
            return new PipelineGraph(pluginGraph, new RootInstanceGraph(pluginGraph), null, pluginGraph.SingletonCache,
                new NulloTransientCache());
        }

        private readonly PluginGraph _pluginGraph;
        private readonly IInstanceGraph _instances;
        private readonly IPipelineGraph _root;
        private readonly IObjectCache _singletons;
        private readonly IObjectCache _transients;
        private readonly Profiles _profiles;

        public PipelineGraph(PluginGraph pluginGraph, IInstanceGraph instances, IPipelineGraph root,
            IObjectCache singletons, IObjectCache transients)
        {
            _pluginGraph = pluginGraph;
            _instances = instances;

            if (root == null)
            {
                _root = this;
                _profiles = new Profiles(_pluginGraph, this);
            }
            else
            {
                _root = root.Root();
                _profiles = root.Profiles;
            }

            _singletons = singletons;
            _transients = transients;
        }

        public IPipelineGraph Root()
        {
            return _root;
        }

        public Profiles Profiles
        {
            get { return _profiles; }
        }

        public IInstanceGraph Instances
        {
            get { return _instances; }
        }

        public IObjectCache Singletons
        {
            get { return _singletons; }
        }

        public IObjectCache Transients
        {
            get { return _transients; }
        }

        public IModel ToModel()
        {
            return new Model(this, _pluginGraph);
        }

        public string Profile
        {
            get { return _pluginGraph.ProfileName; }
        }

        public ContainerRole Role
        {
            get { return _instances.Role; }
        }

        public IGraphEjector Ejector
        {
            get { return new GraphEjector(_pluginGraph, this); }
        }

        public Policies Policies
        {
            get { return _pluginGraph.Root.Policies; }
        }

        public void Dispose()
        {
            _transients.DisposeAndClear();
            _pluginGraph.SafeDispose();
        }

        public void RegisterContainer(IContainer container)
        {
            _pluginGraph.Families[typeof (IContainer)].SetDefault(new ObjectInstance(container));
        }

        public IPipelineGraph ToNestedGraph()
        {
            var nestedPluginGraph = new PluginGraph(Profile + " - Nested");
            nestedPluginGraph.Parent = _pluginGraph;

            var instances = new ComplexInstanceGraph(this, nestedPluginGraph, ContainerRole.Nested);
            return new PipelineGraph(nestedPluginGraph, instances, this, _singletons,
                new NestedContainerTransientObjectCache());
        }

        public void Configure(Action<ConfigurationExpression> configure)
        {
            var registry = new ConfigurationExpression();
            configure(registry);

            var builder = new PluginGraphBuilder(_pluginGraph);
            builder.Add(registry);

            registry.Registries.Each(x => x.As<IPluginGraphConfiguration>().Register(builder));
            registry.Registries.Each(x => builder.Add(x));

            builder.RunConfigurations();
        }

        public ILifecycle DetermineLifecycle(Type pluginType, Instance instance)
        {
            return instance.DetermineLifecycle(_instances.DefaultLifecycleFor(pluginType));
        }
    }
}