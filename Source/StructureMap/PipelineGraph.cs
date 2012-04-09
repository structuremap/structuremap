using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Query;
using StructureMap.Util;

namespace StructureMap
{
    public delegate InstanceFactory MissingFactoryFunction(Type pluginType, ProfileManager profileManager);


    public class PipelineGraph : IDisposable
    {
        private readonly TypeDictionary<IInstanceFactory> _factories
            = new TypeDictionary<IInstanceFactory>();

        private readonly GenericsPluginGraph _genericsGraph = new GenericsPluginGraph();
        private readonly GraphLog _log;
        private readonly ProfileManager _profileManager;
        private readonly IObjectCache _transientCache;

        private MissingFactoryFunction _missingFactory =
            (pluginType, profileManager) => null;

        public PipelineGraph(PluginGraph graph)
        {
            _transientCache = new NulloObjectCache();
            _profileManager = graph.ProfileManager;
            _log = graph.Log;


            graph.PluginFamilies.Where(x => x.IsGenericTemplate).Each(_genericsGraph.AddFamily);
            graph.PluginFamilies.Where(x => !x.IsGenericTemplate).Each(family =>
            {
                var factory = new InstanceFactory(family);
                _factories.Add(family.PluginType, factory);
            });

            
        }

        private PipelineGraph(ProfileManager profileManager, GenericsPluginGraph genericsGraph, GraphLog log)
        {
            _profileManager = profileManager;
            _genericsGraph = genericsGraph;
            _log = log;
            _transientCache = new MainObjectCache();
        }

        public GraphLog Log { get { return _log; } }

        public MissingFactoryFunction OnMissingFactory { set { _missingFactory = value; } }

        public string CurrentProfile { get { return _profileManager.CurrentProfile; } set { _profileManager.CurrentProfile = value; } }

        public void Dispose()
        {
            //might need some locking in here, since _factories is being modified
            if (_factories.ContainsKey(typeof(IContainer)))
            {
                _factories[typeof (IContainer)].AllInstances.Each(x => x.SafeDispose());
            }

            foreach (var factory in _factories)
            {
                factory.Value.Dispose();
            }

            _factories.Clear();
            _profileManager.Dispose();
            _genericsGraph.ClearAll();

            _transientCache.DisposeAndClear();
        }

        public IEnumerable<IPluginTypeConfiguration> GetPluginTypes(IContainer container)
        {
            foreach (PluginFamily family in _genericsGraph.Families)
            {
                yield return new GenericFamilyConfiguration(family);
            }

            foreach (IInstanceFactory factory in _factories.Values.ToArray())
            {
                yield return new InstanceFactoryTypeConfiguration(factory.PluginType, container, this);
            }
        }

        public PipelineGraph ToNestedGraph()
        {
            var clone = new PipelineGraph(_profileManager.Clone(), _genericsGraph.Clone(), _log)
            {
                _missingFactory = _missingFactory
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

        public void ImportFrom(PluginGraph graph)
        {
            foreach (PluginFamily family in graph.PluginFamilies)
            {
                if (family.IsGenericTemplate)
                {
                    _genericsGraph.ImportFrom(family);
                }
                else
                {
                    ForType(family.PluginType).ImportFrom(family);
                }
            }

            _profileManager.ImportFrom(graph.ProfileManager);
        }

        public IInstanceFactory ForType(Type pluginType)
        {
            createFactoryIfMissing(pluginType);
            return _factories[pluginType];
        }

        [Obsolete("Replace this with a Cache")]
        private void createFactoryIfMissing(Type pluginType)
        {
            if (!_factories.ContainsKey(pluginType))
            {
                lock (this)
                {
                    if (!_factories.ContainsKey(pluginType))
                    {
                        InstanceFactory factory = _missingFactory(pluginType, _profileManager);

                        if (factory == null) factory = createFactory(pluginType);
                        registerFactory(pluginType, factory);
                    }
                }
            }
        }

        protected void registerFactory(Type pluginType, InstanceFactory factory)
        {
            _factories.Add(pluginType, factory);
        }

        protected virtual InstanceFactory createFactory(Type pluggedType)
        {
            if (pluggedType.IsGenericType)
            {
                PluginFamily family = _genericsGraph.CreateTemplatedFamily(pluggedType, _profileManager);
                if (family != null) return InstanceFactory.CreateFactoryForFamily(family, _profileManager);
            }

            return InstanceFactory.CreateFactoryForType(pluggedType, _profileManager);
        }

        public virtual Instance GetDefault(Type pluginType)
        {
            // Need to ensure that the factory exists first
            createFactoryIfMissing(pluginType);
            return _profileManager.GetDefault(pluginType) ?? _factories[pluginType].MissingInstance;
        }

        public void SetDefault(Type pluginType, Instance instance)
        {
            createFactoryIfMissing(pluginType);
            ForType(pluginType).AddInstance(instance);
            _profileManager.SetDefault(pluginType, instance);
        }


        public void AddInstance<T>(Instance instance)
        {
            ForType(typeof (T)).AddInstance(instance);
        }

        public void EjectAllInstancesOf<T>()
        {
            EjectAllInstancesOf(typeof (T));
        }

        public void EjectAllInstancesOf(Type pluginType)
        {
            ForType(pluginType).EjectAllInstances();
            _profileManager.EjectAllInstancesOf(pluginType);
        }

        public void Remove(Func<Type, bool> filter)
        {
            _genericsGraph.Families.Where(x => filter(x.PluginType)).ToArray().Each(x => Remove(x.PluginType));
            _factories.Values.Where(x => filter(x.PluginType)).ToArray().Each(x => Remove(x.PluginType));
        }

        public void Remove(Type pluginType)
        {
            EjectAllInstancesOf(pluginType);
            lock (this)
            {
                _factories.Remove(pluginType);
            }
            _genericsGraph.Remove(pluginType);
        }


        public List<Instance> GetAllInstances()
        {
            return _factories.Values.SelectMany(x => x.AllInstances).ToList();
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
            _factories.Values.ToArray().Each(f =>
            {
                f.AllInstances.ToArray().Each(i => action(f.PluginType, i));
            });
        }

        public IObjectCache FindCache(Type pluginType)
        {
            ILifecycle lifecycle = ForType(pluginType).Lifecycle;
            return lifecycle == null
                       ? _transientCache
                       : lifecycle.FindCache();
        }

        public void Remove(Type pluginType, Instance instance)
        {
            ForType(pluginType).RemoveInstance(instance);
            _profileManager.RemoveInstance(pluginType, instance);
        }
    }
}