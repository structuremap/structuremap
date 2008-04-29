using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using StructureMap.Configuration.Mementos;
using StructureMap.Exceptions;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap
{
    /// <summary>
    /// A collection of IInstanceFactory's.
    /// </summary>
    public class InstanceManager : IInstanceManager, IEnumerable, IInstanceCreator
    {
        private readonly Dictionary<Type, IInstanceFactory> _factories;
        private readonly bool _failOnException = true;
        private readonly GenericsPluginGraph _genericsGraph;
        private readonly InterceptorLibrary _interceptorLibrary;
        private readonly ProfileManager _profileManager;

        /// <summary>
        /// Default constructor
        /// </summary>
        public InstanceManager()
        {
            _factories = new Dictionary<Type, IInstanceFactory>();
            _genericsGraph = new GenericsPluginGraph();
            _interceptorLibrary = new InterceptorLibrary();
            _profileManager = new ProfileManager();
        }

        /// <summary>
        /// Creates an InstanceManager from the contents of the pluginGraph.  Fails
        /// on any exceptions found
        /// </summary>
        /// <param name="pluginGraph"></param>
        public InstanceManager(PluginGraph pluginGraph) : this(pluginGraph, true)
        {
        }

        /// <summary>
        /// Constructor to create an InstanceManager that traps exceptions.  Used to diagnose
        /// configuration and runtime errors
        /// </summary>
        /// <param name="pluginGraph">PluginGraph containing the instance and type definitions 
        /// for the InstanceManager</param>
        /// <param name="failOnException">Flags the InstanceManager to fail or trap exceptions</param>
        public InstanceManager(PluginGraph pluginGraph, bool failOnException) : this()
        {
            _failOnException = failOnException;
            _interceptorLibrary = pluginGraph.InterceptorLibrary;
            _profileManager = pluginGraph.ProfileManager;

            if (!pluginGraph.IsSealed)
            {
                pluginGraph.Seal();
            }

            foreach (PluginFamily family in pluginGraph.PluginFamilies)
            {
                if (family.IsGenericTemplate)
                {
                    _genericsGraph.AddFamily(family);
                }
                else
                {
                    registerPluginFamily(family);
                }
            }
        }

        public virtual IInstanceFactory this[Type pluginType]
        {
            get
            {
                // Preprocess a GenericType
                if (pluginType.IsGenericType && !_factories.ContainsKey(pluginType))
                {
                    PluginFamily family = _genericsGraph.CreateTemplatedFamily(pluginType, _profileManager);
                    return registerPluginFamily(family);
                }

                // Create a new InstanceFactory for a Concrete type
                if (!_factories.ContainsKey(pluginType))
                {
                    if (pluginType.IsInterface || pluginType.IsAbstract)
                    {
                        throw new StructureMapException(208, pluginType.FullName);
                    }

                    return getOrCreateFactory(pluginType);
                }

                // Normal usage
                return _factories[pluginType];
            }
            set { _factories[pluginType] = value; }
        }

        public IInstanceFactory this[string pluginTypeName]
        {
            get
            {
                Type type = Type.GetType(pluginTypeName);


                if (type == null)
                {
                    foreach (KeyValuePair<Type, IInstanceFactory> pair in _factories)
                    {
                        if (pair.Value.PluginType.FullName == pluginTypeName)
                        {
                            return pair.Value;
                        }
                    }

                    throw new MissingPluginFamilyException(pluginTypeName);
                }
                else
                {
                    return this[type];
                }
            }
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _factories.Values.GetEnumerator();
        }

        #endregion

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
            // Gotta get the factory first
            IInstanceFactory factory = getOrCreateFactory(typeof (PLUGINTYPE));
            Instance defaultInstance = _profileManager.GetDefault(typeof (PLUGINTYPE));

            ExplicitInstance<PLUGINTYPE> instance = new ExplicitInstance<PLUGINTYPE>(args, defaultInstance);
            return CreateInstance<PLUGINTYPE>(instance);
        }

        public void Inject<PLUGINTYPE>(PLUGINTYPE instance)
        {
            LiteralInstance literalInstance = new LiteralInstance(instance);
            AddInstance<PLUGINTYPE>(literalInstance);
            SetDefault(typeof (PLUGINTYPE), literalInstance);
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

            foreach (T instance in this[typeof (T)].GetAllInstances())
            {
                list.Add(instance);
            }

            return list;
        }

        public void SetDefaultsToProfile(string profile)
        {
            _profileManager.CurrentProfile = profile;
        }

        /// <summary>
        /// Creates the named instance of the PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public object CreateInstance(Type pluginType, string instanceKey)
        {
            IInstanceFactory instanceFactory = getOrCreateFactory(pluginType);
            return instanceFactory.GetInstance(instanceKey);
        }


        /// <summary>
        /// Creates a new object instance of the requested type
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public object CreateInstance(Type pluginType)
        {
            // Need to fetch Factory first to make sure that the Type is "known"
            IInstanceFactory factory = getOrCreateFactory(pluginType);
            
            Instance instance = _profileManager.GetDefault(pluginType);

            if (instance == null)
            {
                throw new StructureMapException(202, pluginType.FullName);
            }

            return factory.Build(instance);
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
            IInstanceFactory factory = getOrCreateFactory(pluginType);
            return factory.Build(instance);
        }

        /// <summary>
        /// Sets the default instance for the PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        public void SetDefault(Type pluginType, Instance instance)
        {
            _profileManager.SetDefault(pluginType, instance);
        }

        /// <summary>
        /// Sets the default instance for the PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        public void SetDefault(Type pluginType, string instanceKey)
        {
            ReferencedInstance reference = new ReferencedInstance(instanceKey);
            _profileManager.SetDefault(pluginType, reference);
        }


        /// <summary>
        /// Attempts to create a new instance of the requested type.  Automatically inserts the default
        /// configured instance for each dependency in the StructureMap constructor function.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object FillDependencies(Type type)
        {
            if (type.IsInterface || type.IsAbstract)
            {
                throw new StructureMapException(230, type.FullName);
            }

            Plugin plugin = Plugin.CreateImplicitPlugin(type);
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
            if (!Plugin.CanBeCast(pluginType, stub.GetType()))
            {
                throw new StructureMapException(220, pluginType.FullName,
                                                stub.GetType().FullName);
            }

            // TODO:  Hack.  We need to do something that makes the Family be there when
            // Profile is set
            getOrCreateFactory(pluginType);
            LiteralInstance instance = new LiteralInstance(stub);
            _profileManager.SetDefault(pluginType, instance);
        }

        public IList GetAllInstances(Type type)
        {
            return getOrCreateFactory(type).GetAllInstances();
        }

        public void AddInstance<T>(Instance instance)
        {
            IInstanceFactory factory = getOrCreateFactory(typeof (T));
            factory.AddInstance(instance);
        }

        public void AddInstance<PLUGINTYPE, CONCRETETYPE>() where CONCRETETYPE : PLUGINTYPE
        {
            IInstanceFactory factory = getOrCreateFactory(typeof (PLUGINTYPE));
            Instance instance = factory.AddType<CONCRETETYPE>();
            factory.AddInstance(instance);
        }

        public void AddDefaultInstance<PLUGINTYPE, CONCRETETYPE>()
        {
            IInstanceFactory factory = getOrCreateFactory(typeof (PLUGINTYPE));
            Instance instance = factory.AddType<CONCRETETYPE>();
            _profileManager.SetDefault(typeof(PLUGINTYPE), instance);
        }

        public string WhatDoIHave()
        {
            StringBuilder sb = new StringBuilder();

            throw new NotImplementedException("Redo");
            //foreach (IInstanceFactory factory in this)
            //{
            //    sb.AppendFormat("PluginType {0}, Default: {1}\r\n", factory.PluginType.AssemblyQualifiedName,
            //                    factory.DefaultInstanceKey);
            //}

            return sb.ToString();
        }

        #endregion

        private IInstanceFactory registerPluginFamily(PluginFamily family)
        {
            InstanceFactory factory = new InstanceFactory(family, _failOnException);
            RegisterType(factory);

            return factory;
        }


        /// <summary>
        /// Adds an instance of an IInstanceFactory
        /// </summary>
        /// <param name="instanceFactory"></param>
        public void RegisterType(IInstanceFactory instanceFactory)
        {
            _factories.Add(instanceFactory.PluginType, instanceFactory);
            instanceFactory.SetInstanceManager(this);
        }

        /// <summary>
        /// Creates an array of object instances of the requested type
        /// </summary>
        /// <param name="pluginTypeName"></param>
        /// <param name="instances"></param>
        /// <returns></returns>
        // TODO -- make this use a Type!
        Array IInstanceCreator.CreateInstanceArray(string pluginTypeName, Instance[] instances)
        {
            // TODO -- default to returning all
            if (instances == null)
            {
                throw new StructureMapException(205, pluginTypeName, "UNKNOWN");
            }

            Type pluginType = Type.GetType(pluginTypeName);
            Array array = Array.CreateInstance(pluginType, instances.Length);
            for (int i = 0; i < instances.Length; i++)
            {
                Instance instance = instances[i];
                object arrayValue = CreateInstance(pluginType, instance);
                array.SetValue(arrayValue, i);
            }

            return array;
        }

        protected IInstanceFactory getOrCreateFactory(Type type)
        {
            if (!_factories.ContainsKey(type))
            {
                lock (this)
                {
                    if (!_factories.ContainsKey(type))
                    {
                        InstanceFactory factory = createFactory(type);
                        factory.SetInstanceManager(this);
                        _factories.Add(type, factory);
                    }
                }
            }

            return _factories[type];
        }

        protected virtual InstanceFactory createFactory(Type pluggedType)
        {
            if (pluggedType.IsGenericType)
            {
                PluginFamily family = _genericsGraph.CreateTemplatedFamily(pluggedType, _profileManager);
                if (family != null) return new InstanceFactory(family, true);
            }

            return InstanceFactory.CreateInstanceFactoryForType(pluggedType, _profileManager);
        }

        #region Nested type: CreateFactoryDelegate

        protected delegate InstanceFactory CreateFactoryDelegate(Type type);

        #endregion


        object IInstanceCreator.ApplyInterception(Type pluginType, object actualValue)
        {
            IInstanceFactory factory = getOrCreateFactory(pluginType);
            object interceptedValue = factory.ApplyInterception(actualValue);

            return _interceptorLibrary.FindInterceptor(interceptedValue.GetType()).Process(interceptedValue);
        }

        object IInstanceCreator.CreateInstance(Type pluginType, IConfiguredInstance instance)
        {
            return getOrCreateFactory(pluginType).GetInstance(instance, this);
        }
    }
}