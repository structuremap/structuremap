using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Query;
using StructureMap.TypeRules;

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
            ITransientTracking transients = pluginGraph.TransientTracking == TransientTracking.DefaultNotTrackedAtRoot
                ? (ITransientTracking) new NulloTransientCache()
                : new TrackingTransientCache();

            return new PipelineGraph(pluginGraph, new RootInstanceGraph(pluginGraph), null, pluginGraph.SingletonCache,
                transients);
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
        private readonly ITransientTracking _transients;
        private readonly Profiles _profiles;
        private bool _wasDisposed;

        public PipelineGraph(PluginGraph pluginGraph, IInstanceGraph instances, IPipelineGraph root, IObjectCache singletons, ITransientTracking transients)
        {
            _pluginGraph = pluginGraph;
            _instances = instances;

            if (root == null)
            {
                _root = this;
                _profiles = new Profiles(_pluginGraph, this);
                ContainerCache = singletons;
            }
            else
            {
                _root = root.Root();
                _profiles = root.Profiles;
                ContainerCache = new ContainerSpecificObjectCache();
            }

            _singletons = singletons;
            _transients = transients;
        }


        public IObjectCache ContainerCache { get; private set; }

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

        public ITransientTracking Transients
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
            get
            {
                return _pluginGraph.Root.Policies;
            }
        }

        public void Dispose()
        {
            if (_wasDisposed) return;
            _wasDisposed = true;

            if (Role == ContainerRole.Root)
            {
                _singletons.DisposeAndClear();
            }
            
            
            if (Role == ContainerRole.ProfileOrChild)
            {
                ContainerCache.DisposeAndClear();
            }

            _transients.DisposeAndClear();
            _pluginGraph.SafeDispose();

            _profiles.AllProfiles().Each(x => x.Dispose());
        }

        public void RegisterContainer(IContainer container)
        {
            var containerInstance = new ObjectInstance(container);
            containerInstance.SetLifecycleTo<TransientLifecycle>();
            _pluginGraph.Families[typeof (IContainer)].SetDefault(containerInstance);
        }

        public IPipelineGraph ToNestedGraph()
        {
            var nestedPluginGraph = new PluginGraph(Profile + " - Nested") {Parent = _pluginGraph};

            var instances = new ComplexInstanceGraph(this, nestedPluginGraph, ContainerRole.Nested);
            return new PipelineGraph(nestedPluginGraph, instances, this, _singletons,
                new ContainerSpecificObjectCache());
        }

        public void Configure(Action<ConfigurationExpression> configure)
        {
            var registry = new ConfigurationExpression();
            configure(registry);

            if (registry.HasPolicyChanges() && Role == ContainerRole.Nested)
            {
                throw new StructureMapConfigurationException("Policy changes to a nested container are not allowed. Policies can only be applied to the root container");
            }

            var builder = new PluginGraphBuilder(_pluginGraph);
            builder.Add(registry);

            registry.Registries.Each(x => x.As<IPluginGraphConfiguration>().Register(builder));
            registry.Registries.Each(x => builder.Add(x));

            builder.RunConfigurations();

            if (registry.HasPolicyChanges())
            {
                

                Instances.GetAllInstances().ToArray().Each(x => x.ClearBuildPlan());

                Profiles.AllProfiles().ToArray()
                    .Each(x => x.Instances.GetAllInstances().ToArray().Each(i => i.ClearBuildPlan()));

            }
        }

        public void ValidateValidNestedScoping()
        {
            var descriptions = new List<string>();

            _pluginGraph.Families.Each(family => {
                family.Instances.Where(x => !(x is IValue)).Each(instance => {
                    var lifecycle = instance.DetermineLifecycle(family.Lifecycle);
                    if (!(lifecycle is IAppropriateForNestedContainer))
                    {
                        descriptions.Add("{0} or plugin type {1} has lifecycle {2}".ToFormat(instance.Description, family.PluginType.GetFullName(), lifecycle.Description));
                    }
                });
            });

            if (!descriptions.Any()) return;

            var message =
                "Only registrations of the default Transient, UniquePerRequest, and prebuilt objects are valid for nested containers.  Remember that 'Transient' instances will be built once per nested container.  If you need this functionality, try using a Child/Profile container instead\n";
            message += string.Join("\n", descriptions);

            throw new InvalidOperationException(message);
        }

        public ILifecycle DetermineLifecycle(Type pluginType, Instance instance)
        {
            return instance.DetermineLifecycle(_instances.DefaultLifecycleFor(pluginType));
        }
    }
}