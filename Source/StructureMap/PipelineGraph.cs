using System;
using System.Collections.Generic;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    public delegate InstanceFactory MissingFactoryFunction(Type pluginType, ProfileManager profileManager);

    public interface IPipelineGraphVisitor
    {
        void PluginType(Type pluginType, Instance defaultInstance);
        void Instance(Type pluginType, Instance instance);
    }

    public class PipelineGraph
    {
        private readonly Dictionary<Type, IInstanceFactory> _factories
            = new Dictionary<Type, IInstanceFactory>();

        private readonly GenericsPluginGraph _genericsGraph = new GenericsPluginGraph();
        private readonly ProfileManager _profileManager;

        private MissingFactoryFunction _missingFactory =
            (pluginType, profileManager) => null;

        private GraphLog _log;

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
                    InstanceFactory factory = new InstanceFactory(family);
                    _factories.Add(family.PluginType, factory);
                }
            }
        }

        public GraphLog Log
        {
            get { return _log; }
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

        public MissingFactoryFunction OnMissingFactory
        {
            set { _missingFactory = value; }
        }

        public string CurrentProfile
        {
            get { return _profileManager.CurrentProfile; }
            set { _profileManager.CurrentProfile = value; }
        }

        public void Visit(IPipelineGraphVisitor visitor)
        {
            foreach (KeyValuePair<Type, IInstanceFactory> pair in _factories)
            {
                Type pluginType = pair.Value.PluginType;
                Instance defaultInstance = _profileManager.GetDefault(pluginType);

                visitor.PluginType(pluginType, defaultInstance);
                pair.Value.ForEachInstance(instance => visitor.Instance(pluginType, instance));
            }


        }

        // Useful for the validation logic
        public List<Instance> GetAllInstances()
        {
            PipelineGraphFlattener flattener = new PipelineGraphFlattener();
            Visit(flattener);

            return flattener.Instances;
        }

        internal class PipelineGraphFlattener : IPipelineGraphVisitor
        {
            internal List<Instance> Instances = new List<Instance>();

            public void PluginType(Type pluginType, Instance defaultInstance){}

            public void Instance(Type pluginType, Instance instance)
            {
                Instances.Add(instance);
            }
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

        public Instance GetDefault(Type pluginType)
        {
            // Need to ensure that the factory exists first
            createFactoryIfMissing(pluginType);
            return _profileManager.GetDefault(pluginType);
        }

        public void SetDefault(Type pluginType, Instance instance)
        {
            createFactoryIfMissing(pluginType);
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
            LiteralInstance literalInstance = new LiteralInstance(instance);
            ForType(typeof (PLUGINTYPE)).AddInstance(literalInstance);
            SetDefault(typeof (PLUGINTYPE), literalInstance);
        }
    }
}