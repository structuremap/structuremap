using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap
{
    /// <summary>
    /// A collection of IInstanceFactory's.
    /// </summary>
    public class InstanceManager : TypeRules, IInstanceManager
    {
        private readonly InterceptorLibrary _interceptorLibrary;
        private readonly PipelineGraph _pipelineGraph;

        public InstanceManager() : this(new PluginGraph())
        {
        }

        /// <summary>
        /// Constructor to create an InstanceManager
        /// </summary>
        /// <param name="pluginGraph">PluginGraph containing the instance and type definitions 
        /// for the InstanceManager</param>
        /// <param name="failOnException">Flags the InstanceManager to fail or trap exceptions</param>
        public InstanceManager(PluginGraph pluginGraph)
        {
            _interceptorLibrary = pluginGraph.InterceptorLibrary;

            if (!pluginGraph.IsSealed)
            {
                pluginGraph.Seal();
            }

            _pipelineGraph = new PipelineGraph(pluginGraph);
        }

        protected MissingFactoryFunction onMissingFactory
        {
            set { _pipelineGraph.OnMissingFactory = value; }
        }

        #region IInstanceManager Members

        public T CreateInstance<T>(string instanceKey)
        {
            return (T) CreateInstance(typeof (T), instanceKey);
        }

        public T CreateInstance<T>(Instance instance)
        {
            return (T) CreateInstance(typeof (T), instance);
        }

        public PLUGINTYPE CreateInstance<PLUGINTYPE>(ExplicitArguments args)
        {
            Instance defaultInstance = _pipelineGraph.GetDefault(typeof (PLUGINTYPE));

            ExplicitInstance<PLUGINTYPE> instance = new ExplicitInstance<PLUGINTYPE>(args, defaultInstance);
            return CreateInstance<PLUGINTYPE>(instance);
        }

        public void Inject<PLUGINTYPE>(PLUGINTYPE instance)
        {
            _pipelineGraph.Inject(instance);
        }

        public void InjectByName<PLUGINTYPE>(PLUGINTYPE instance, string instanceKey)
        {
            LiteralInstance literalInstance = new LiteralInstance(instance);
            literalInstance.Name = instanceKey;

            AddInstance<PLUGINTYPE>(literalInstance);
        }

        public void InjectByName<PLUGINTYPE, CONCRETETYPE>(string instanceKey)
        {
            ConfiguredInstance instance = new ConfiguredInstance();
            instance.PluggedType = typeof(CONCRETETYPE);
            instance.Name = instanceKey;

            AddInstance<PLUGINTYPE>(instance);
        }

        public T CreateInstance<T>()
        {
            return (T) CreateInstance(typeof (T));
        }

        public T FillDependencies<T>()
        {
            return (T) FillDependencies(typeof (T));
        }

        public void InjectStub<T>(T instance)
        {
            InjectStub(typeof (T), instance);
        }

        public IList<T> GetAllInstances<T>()
        {
            List<T> list = new List<T>();

            IBuildSession session = withNewSession();

            foreach (T instance in forType(typeof (T)).GetAllInstances(session))
            {
                list.Add(instance);
            }

            return list;
        }

        public void SetDefaultsToProfile(string profile)
        {
            _pipelineGraph.CurrentProfile = profile;
        }

        /// <summary>
        /// Creates the named instance of the PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public object CreateInstance(Type pluginType, string instanceKey)
        {
            return withNewSession().CreateInstance(pluginType, instanceKey);
        }


        /// <summary>
        /// Creates a new object instance of the requested type
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public object CreateInstance(Type pluginType)
        {
            return withNewSession().CreateInstance(pluginType);
        }


        /// <summary>
        /// Creates a new instance of the requested type using the InstanceMemento.  Mostly used from other
        /// classes to link children members
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public object CreateInstance(Type pluginType, Instance instance)
        {
            return withNewSession().CreateInstance(pluginType, instance);
        }

        /// <summary>
        /// Sets the default instance for the PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        public void SetDefault(Type pluginType, Instance instance)
        {
            _pipelineGraph.SetDefault(pluginType, instance);
        }

        /// <summary>
        /// Sets the default instance for the PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        public void SetDefault(Type pluginType, string instanceKey)
        {
            ReferencedInstance reference = new ReferencedInstance(instanceKey);
            _pipelineGraph.SetDefault(pluginType, reference);
        }


        /// <summary>
        /// Attempts to create a new instance of the requested type.  Automatically inserts the default
        /// configured instance for each dependency in the StructureMap constructor function.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object FillDependencies(Type type)
        {
            if (!IsConcrete(type))
            {
                throw new StructureMapException(230, type.FullName);
            }

            // TODO:  Little bit of smelliness here
            Plugin plugin = new Plugin(type);
            if (!plugin.CanBeAutoFilled)
            {
                throw new StructureMapException(230, type.FullName);
            }

            return CreateInstance(type);
        }

        /// <summary>
        /// Sets up the InstanceManager to return the object in the "stub" argument anytime
        /// any instance of the PluginType is requested
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="stub"></param>
        public void InjectStub(Type pluginType, object stub)
        {
            if (!CanBeCast(pluginType, stub.GetType()))
            {
                throw new StructureMapException(220, pluginType.FullName,
                                                stub.GetType().FullName);
            }


            LiteralInstance instance = new LiteralInstance(stub);
            _pipelineGraph.SetDefault(pluginType, instance);
        }

        public IList GetAllInstances(Type type)
        {
            return forType(type).GetAllInstances(withNewSession());
        }

        public void AddInstance<T>(Instance instance)
        {
            _pipelineGraph.AddInstance<T>(instance);
        }

        public void AddInstance<PLUGINTYPE, CONCRETETYPE>() where CONCRETETYPE : PLUGINTYPE
        {
            _pipelineGraph.AddInstance<PLUGINTYPE, CONCRETETYPE>();
        }

        public void AddDefaultInstance<PLUGINTYPE, CONCRETETYPE>()
        {
            _pipelineGraph.AddDefaultInstance<PLUGINTYPE, CONCRETETYPE>();
        }

        public string WhatDoIHave()
        {
            WhatDoIHaveWriter writer = new WhatDoIHaveWriter(_pipelineGraph);
            return writer.GetText();
        }

        #endregion

        private IBuildSession withNewSession()
        {
            return new BuildSession(_pipelineGraph, _interceptorLibrary);
        }


        protected IInstanceFactory forType(Type type)
        {
            return _pipelineGraph.ForType(type);
        }
    }
}