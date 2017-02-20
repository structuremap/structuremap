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
            var transients = chooseTransientTracking(pluginGraph);

            return new PipelineGraph(pluginGraph, new RootInstanceGraph(pluginGraph), null, pluginGraph.SingletonCache,
                transients);
        }

        private static ITransientTracking chooseTransientTracking(PluginGraph pluginGraph)
        {
            ITransientTracking transients = pluginGraph.TransientTracking == TransientTracking.DefaultNotTrackedAtRoot
                ? (ITransientTracking) new NulloTransientCache()
                : new TrackingTransientCache();
            return transients;
        }

        public static IPipelineGraph BuildEmpty()
        {
            var pluginGraph = PluginGraph.CreateRoot();
            return new PipelineGraph(pluginGraph, new RootInstanceGraph(pluginGraph), null, pluginGraph.SingletonCache,
                new NulloTransientCache());
        }

        private readonly PluginGraph _pluginGraph;
        private readonly IInstanceGraph _instances;
        private readonly IPipelineGraph _root;
        private readonly IObjectCache _singletons;
        private ITransientTracking _transients;
        private readonly Profiles _profiles;
        private bool _wasDisposed;
        private readonly IList<IDisposable> _trackedDisposables = new List<IDisposable>(); 

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

            ContainerCache.DisposeAndClear();
            

            _transients.DisposeAndClear();
            _pluginGraph.SafeDispose();

            if (Role == ContainerRole.Root)
            {
                _profiles.AllProfiles().Each(x => x.Dispose());
            }

            _trackedDisposables.Each(x => x.Dispose());
        }

        public void RegisterContainer(IContainer container)
        {
            var containerInstance = new ObjectInstance(container);
            containerInstance.SetLifecycleTo<TransientLifecycle>();
            _pluginGraph.Families[typeof (IContainer)].SetDefault(containerInstance);
        }

        public IPipelineGraph ToNestedGraph(TypeArguments arguments = null)
        {
            var nestedPluginGraph = _pluginGraph.ToNestedGraph();
            if (arguments != null)
            {
                foreach (var pair in arguments.Defaults)
                {
                    nestedPluginGraph.Families[pair.Key] = PluginFamily.ExplicitOverride(pair.Key, pair.Value);
                }
            }

            var instances = new ComplexInstanceGraph(this, nestedPluginGraph, ContainerRole.Nested);
            return new PipelineGraph(nestedPluginGraph, instances, this, _singletons,
                new ContainerSpecificObjectCache());
        }

        public void Configure(Action<ConfigurationExpression> configure)
        {
            if (_pluginGraph.IsRunningConfigure)
                throw new StructureMapConfigurationException("The container is already being configured. Recursive IContainer.Configure() calls are not allowed");

            _pluginGraph.IsRunningConfigure = true;

            try
            {
                var registry = new ConfigurationExpression();
                configure(registry);

                var transientTracking = registry.GetTransientTracking();
                if (transientTracking != null &&
                    transientTracking != _pluginGraph.TransientTracking)
                {
                    changeTransientTracking(transientTracking.Value);
                }

                if (registry.HasPolicyChanges() && Role == ContainerRole.Nested)
                {
                    throw new StructureMapConfigurationException("Policy changes to a nested container are not allowed. Policies can only be applied to the root container");
                }

                if (registry.HasPolicyChanges())
                {
                    _pluginGraph.ClearTypeMisses();
                }

                var builder = new PluginGraphBuilder(_pluginGraph);
                builder.Add(registry);

                registry.Registries.Each(x => builder.Add(x));

                builder.RunConfigurations();

                if (registry.HasPolicyChanges())
                {
                    Instances.GetAllInstances().ToArray().Each(x => x.ClearBuildPlan());

                    Profiles.AllProfiles().ToArray()
                        .Each(x => x.Instances.GetAllInstances().ToArray().Each(i => i.ClearBuildPlan()));

                }
            }
            finally
            {
                _pluginGraph.IsRunningConfigure = false;
            }
        }

        private void changeTransientTracking(TransientTracking transientTracking)
        {
            _pluginGraph.TransientTracking = transientTracking;
            _transients = chooseTransientTracking(_pluginGraph);
        }

        public void ValidateValidNestedScoping()
        {
            var descriptions = new List<string>();

            _pluginGraph.Families.ToArray().Each(family => {
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

        public void TrackDisposable(IDisposable disposable)
        {
            _trackedDisposables.Add(disposable);
        }

        public ILifecycle DetermineLifecycle(Type pluginType, Instance instance)
        {
            // Need to force the build plan to be created here.
            if (!instance.HasBuildPlan())
            {
                instance.ApplyAllPolicies(pluginType, Policies);
            }
            

            return instance.DetermineLifecycle(_instances.DefaultLifecycleFor(pluginType));
        }
    }
}