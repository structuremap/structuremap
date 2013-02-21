using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using StructureMap.Query;
using StructureMap.TypeRules;

namespace StructureMap
{
    public class PipelineGraph : IDisposable, IPipelineGraph, IGraphEjector
    {
        private readonly Dictionary<Type, IInstanceFactory> _factories
            = new Dictionary<Type, IInstanceFactory>();

        private readonly GraphLog _log;
        private readonly ProfileManager _profileManager;
        private readonly IObjectCache _transientCache;

        private PluginGraph _graph;
        private InterceptorLibrary _interceptors;

        public PipelineGraph(PluginGraph graph)
        {
            _transientCache = new NulloTransientCache();
            _profileManager = graph.ProfileManager;
            _log = graph.Log;


            graph.Families.Where(x => !x.IsGenericTemplate).Each(family => {
                var factory = new InstanceFactory(family);
                _factories.Add(family.PluginType, factory);
            });

            _interceptors = graph.InterceptorLibrary;

            _graph = graph;
        }

        private PipelineGraph(ProfileManager profileManager, GraphLog log)
        {
            _profileManager = profileManager;
            _log = log;
            _transientCache = new NestedContainerTransientObjectCache();
        }

        public GraphLog Log
        {
            get { return _log; }
        }

        public string CurrentProfile
        {
            get { return _profileManager.CurrentProfile; }
            set { _profileManager.CurrentProfile = value; }
        }

        public void Dispose()
        {
            //might need some locking in here, since _factories is being modified
            if (_factories.ContainsKey(typeof (IContainer)))
            {
                _factories[typeof (IContainer)].AllInstances.Each(x => x.SafeDispose());
            }

            foreach (var factory in _factories)
            {
                factory.Value.Dispose();
            }

            _factories.Clear();
            _profileManager.Dispose();

            _transientCache.DisposeAndClear();
        }

        public IPipelineGraph ForProfile(string profile)
        {
            CurrentProfile = profile;
            return this;
        }

        public IEnumerable<IPluginTypeConfiguration> GetPluginTypes(IContainer container)
        {
            foreach (PluginFamily family in _graph.Families.Where(x => x.PluginType.IsOpenGeneric()))
            {
                yield return new GenericFamilyConfiguration(family);
            }

            foreach (IInstanceFactory factory in _factories.Values.ToArray())
            {
                yield return new InstanceFactoryTypeConfiguration(factory.PluginType, container, this);
            }
        }

        public IPipelineGraph ToNestedGraph()
        {
            var clone = new PipelineGraph(_profileManager.Clone(), _log)
            {
                _interceptors = _interceptors,
                _graph = _graph
            };

            lock (this)
            {
                foreach (var pair in _factories)
                {
                    clone._factories.Add(pair.Key, pair.Value.Clone());
                }
            }

            clone.EjectAllInstancesOf<IContainer>();

            return clone;
        }

        public IEnumerable<Type> AllPluginTypes()
        {
            throw new NotImplementedException();
        }

        [Obsolete("HATE this")]
        public void ImportFrom(PluginGraph graph)
        {
            foreach (PluginFamily family in graph.Families)
            {
                if (family.IsGenericTemplate)
                {
                    graph.AddFamily(family); // TODO -- this is awful
                }
                else
                {
                    ForType(family.PluginType).ImportFrom(family);
                }
            }

            _profileManager.ImportFrom(graph.ProfileManager);
        }

        public IPipelineGraph Root()
        {
            return new PipelineGraph(_graph);
        }

        public InstanceInterceptor FindInterceptor(Type concreteType)
        {
            return _interceptors.FindInterceptor(concreteType);
        }

        public virtual Instance GetDefault(Type pluginType)
        {
            // Need to ensure that the factory exists first
            createFactoryIfMissing(pluginType);
            Instance instance = _profileManager.GetDefault(pluginType);
            if (instance == null && pluginType.IsGenericType)
                instance = _profileManager.GetDefault(pluginType.GetGenericTypeDefinition());

            // if we haven't found the instance yet, return it. If we're using an auto-mocking
            // container, that will handle the missing instance at this point.
            return instance ?? _factories[pluginType].MissingInstance;
        }

        public void EjectAllInstancesOf<T>()
        {
            EjectAllInstancesOf(typeof (T));
        }

        public void Remove(Func<Type, bool> filter)
        {
            _graph.Families.Where(x => filter(x.PluginType)).ToArray().Each(x => _graph.RemoveFamily(x.PluginType));

            _factories.Values.Where(x => filter(x.PluginType)).ToArray().Each(x => Remove(x.PluginType));
        }

        public void Remove(Type pluginType)
        {
            EjectAllInstancesOf(pluginType);
            lock (this)
            {
                _factories.Remove(pluginType);
            }

            _graph.RemoveFamily(pluginType);
        }


        public IEnumerable<Instance> GetAllInstances()
        {
            return _factories.Values.SelectMany(x => x.AllInstances).ToList();
        }

        public IEnumerable<Instance> GetAllInstances(Type pluginType)
        {
            return ForType(pluginType).AllInstances;
        }

        public Instance FindInstance(Type pluginType, string name)
        {
            return ForType(pluginType).FindInstance(name);
        }

        public bool HasDefaultForPluginType(Type pluginType)
        {
            IInstanceFactory factory = ForType(pluginType);
            if (_profileManager.GetDefault(pluginType) != null)
            {
                return true;
            }

            return (factory.AllInstances.Count() == 1);
        }

        public bool HasInstance(Type pluginType, string instanceKey)
        {
            return ForType(pluginType).FindInstance(instanceKey) != null;
        }

        public void EachInstance(Action<Type, Instance> action)
        {
            _factories.Values.ToArray().Each(f => { f.AllInstances.ToArray().Each(i => action(f.PluginType, i)); });
        }

        public IObjectCache Singletons
        {
            get { return _graph.SingletonCache; }
        }

        public IObjectCache Transients
        {
            get { return _transientCache; }
        }

        public IInstanceFactory ForType(Type pluginType)
        {
            createFactoryIfMissing(pluginType);
            return _factories[pluginType];
        }

        [Obsolete("Replace this with a Cache?")]
        private void createFactoryIfMissing(Type pluginType)
        {
            if (!_factories.ContainsKey(pluginType))
            {
                lock (this)
                {
                    if (!_factories.ContainsKey(pluginType))
                    {
                        var family = _graph.Families[pluginType];
                        var @default = family.GetDefaultInstance();

                        var factory = new InstanceFactory(family);
                        if (@default != null) _profileManager.SetDefault(pluginType, @default);

                        registerFactory(pluginType, factory);
                    }
                }
            }
        }

        protected void registerFactory(Type pluginType, InstanceFactory factory)
        {
            _factories.Add(pluginType, factory);
        }

        public void AddInstance<T>(Instance instance)
        {
            ForType(typeof (T)).AddInstance(instance);
        }

        public void EjectAllInstancesOf(Type pluginType)
        {
            ForType(pluginType).EjectAllInstances(this);

            if (_graph.Families.Has(pluginType))
            {
                _graph.Families[pluginType].RemoveAll();
            }

            _profileManager.EjectAllInstancesOf(pluginType);
        }

        public bool IsUnique(Type pluginType)
        {
            return ForType(pluginType).Lifecycle is UniquePerRequestLifecycle;
        }

        public void Remove(Type pluginType, Instance instance)
        {
            ForType(pluginType).RemoveInstance(instance);
            _profileManager.RemoveInstance(pluginType, instance);
        }

        public IGraphEjector Ejector
        {
            get { return this; }
        }
    }
}