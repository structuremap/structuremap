using System;
using System.Collections.Generic;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    public delegate InstanceFactory MissingFactoryFunction(Type pluginType, ProfileManager profileManager);


    public class PluginTypeConfiguration
    {
        public Type PluginType { get; set; }
        public IInstance Default { get; set; }
        public IBuildPolicy Policy { get; set; }

        public IEnumerable<IInstance> Instances { get; set; }
    }

    public class PipelineGraph
    {
        private readonly Dictionary<Type, IInstanceFactory> _factories
            = new Dictionary<Type, IInstanceFactory>();

        private readonly GenericsPluginGraph _genericsGraph = new GenericsPluginGraph();
        private readonly GraphLog _log;
        private readonly ProfileManager _profileManager;

        private MissingFactoryFunction _missingFactory =
            (pluginType, profileManager) => null;

        public PipelineGraph(PluginGraph graph)
        {
            _profileManager = graph.ProfileManager;
            _log = graph.Log;

            foreach (PluginFamily family in graph.PluginFamilies)
            {
                if (family.IsGenericTemplate)
                {
                    _genericsGraph.AddFamily(family);
                }
                else
                {
                    var factory = new InstanceFactory(family);
                    _factories.Add(family.PluginType, factory);
                }
            }
        }

        public GraphLog Log
        {
            get { return _log; }
        }

        public MissingFactoryFunction OnMissingFactory
        {
            set { _missingFactory = value; }
        }

        public string CurrentProfile
        {
            get { return _profileManager.CurrentProfile; }
            set { _profileManager.CurrentProfile = value; }
        }

        public IEnumerable<PluginTypeConfiguration> PluginTypes
        {
            get
            {
                foreach (PluginTypeConfiguration configuration in _genericsGraph.Families)
                {
                    yield return configuration;
                }

                IInstanceFactory[] factories = new IInstanceFactory[_factories.Count];
                _factories.Values.CopyTo(factories, 0);

                foreach (var factory in factories)
                {
                    yield return new PluginTypeConfiguration
                                     {
                                         Default = _profileManager.GetDefault(factory.PluginType),
                                         PluginType = factory.PluginType,
                                         Policy = factory.Policy,
                                         Instances = factory.Instances
                                     };
                }
            }
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
                if (family != null) return new InstanceFactory(family);
            }

            return InstanceFactory.CreateFactoryForType(pluggedType, _profileManager);
        }

        public virtual Instance GetDefault(Type pluginType)
        {
            // Need to ensure that the factory exists first
            createFactoryIfMissing(pluginType);
            return _profileManager.GetDefault(pluginType);
        }

        public void SetDefault(Type pluginType, Instance instance)
        {
            createFactoryIfMissing(pluginType);
            ForType(pluginType).AddInstance(instance);
            _profileManager.SetDefault(pluginType, instance);
        }

        public Instance AddInstance<PLUGINTYPE, PLUGGEDTYPE>()
        {
            return ForType(typeof (PLUGINTYPE)).AddType<PLUGGEDTYPE>();
        }

        public void AddInstance<T>(Instance instance)
        {
            ForType(typeof (T)).AddInstance(instance);
        }

        public void AddDefaultInstance<PLUGINTYPE, PLUGGEDTYPE>()
        {
            Instance instance = AddInstance<PLUGINTYPE, PLUGGEDTYPE>();
            _profileManager.SetDefault(typeof (PLUGINTYPE), instance);
        }

        public void Inject<PLUGINTYPE>(PLUGINTYPE instance)
        {
            var literalInstance = new LiteralInstance(instance);
            ForType(typeof (PLUGINTYPE)).AddInstance(literalInstance);
            SetDefault(typeof (PLUGINTYPE), literalInstance);
        }

        public void EjectAllInstancesOf<T>()
        {
            ForType(typeof (T)).EjectAllInstances();
            _profileManager.EjectAllInstancesOf<T>();
        }

        public IEnumerable<IInstance> InstancesOf(Type pluginType)
        {
            if (_genericsGraph.HasFamily(pluginType))
            {
                return _genericsGraph.FindFamily(pluginType).Instances;
            }

            if (_factories.ContainsKey(pluginType))
            {
                return _factories[pluginType].Instances;
            }

            return new IInstance[0];
        }

        public List<IInstance> GetAllInstances()
        {
            var list = new List<IInstance>();

            foreach (var pair in _factories)
            {
                list.AddRange(pair.Value.Instances);
            }

            return list;
        }
    }
}