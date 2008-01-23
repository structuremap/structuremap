using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using StructureMap.Configuration.Mementos;
using StructureMap.Exceptions;
using StructureMap.Graph;
using StructureMap.Interceptors;

namespace StructureMap
{
    /// <summary>
    /// A collection of IInstanceFactory's.
    /// </summary>
    public class InstanceManager : IInstanceManager, IEnumerable
    {
        private readonly InstanceDefaultManager _defaultManager;
        private readonly Dictionary<Type, IInstanceFactory> _factories;
        private readonly bool _failOnException = true;
        private readonly GenericsPluginGraph _genericsGraph;
        private readonly InterceptorLibrary _interceptorLibrary;

        /// <summary>
        /// Default constructor
        /// </summary>
        public InstanceManager()
        {
            _factories = new Dictionary<Type, IInstanceFactory>();
            _genericsGraph = new GenericsPluginGraph();
            _interceptorLibrary = new InterceptorLibrary();
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
            _defaultManager = pluginGraph.DefaultManager;
            _interceptorLibrary = pluginGraph.InterceptorLibrary;

            if (!pluginGraph.IsSealed)
            {
                pluginGraph.Seal();
            }

            foreach (PluginFamily family in pluginGraph.PluginFamilies)
            {
                if (family.IsGeneric)
                {
                    _genericsGraph.AddFamily(family);
                }
                else
                {
                    registerPluginFamily(family);
                }
            }
        }


        public InterceptorLibrary InterceptorLibrary
        {
            get { return _interceptorLibrary; }
        }

        public virtual IInstanceFactory this[Type pluginType]
        {
            get
            {
                // Preprocess a GenericType
                if (pluginType.IsGenericType && !_factories.ContainsKey(pluginType))
                {
                    PluginFamily family = _genericsGraph.CreateTemplatedFamily(pluginType);
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

        public InstanceDefaultManager DefaultManager
        {
            get { return _defaultManager; }
        }

        public T CreateInstance<T>(string instanceKey)
        {
            return (T) CreateInstance(typeof (T), instanceKey);
        }

        public T CreateInstance<T>(InstanceMemento memento)
        {
            return (T) CreateInstance(typeof (T), memento);
        }

        public PLUGINTYPE CreateInstance<PLUGINTYPE>(ExplicitArguments args)
        {
            ExplicitArgumentMemento memento = new ExplicitArgumentMemento(args, null);
            return CreateInstance<PLUGINTYPE>(memento);
            
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
            // The authenticated user may not have required privileges to read from Environment
            string machineName = InstanceDefaultManager.GetMachineName();
            Profile defaultProfile = _defaultManager.CalculateDefaults(machineName, profile);
            SetDefaults(defaultProfile);
        }

        #endregion

        private IInstanceFactory registerPluginFamily(PluginFamily family)
        {
            InstanceFactory factory = new InstanceFactory(family, _failOnException);
            IInstanceFactory wrappedFactory = family.InterceptionChain.WrapInstanceFactory(factory);

            RegisterType(wrappedFactory);

            return wrappedFactory;
        }

        /// <summary>
        /// Sets the default instances for all PluginType's managed by the InstanceManager
        /// </summary>
        /// <param name="defaultProfile"></param>
        public void SetDefaults(Profile defaultProfile)
        {
            foreach (InstanceDefault defaultInstance in defaultProfile.Defaults)
            {
                try
                {
                    string pluginTypeName = defaultInstance.PluginTypeName;
                    PluginFamily genericFamily = _genericsGraph.FindGenericFamily(pluginTypeName);
                    if (genericFamily == null)
                    {
                        IInstanceFactory instanceFactory = this[pluginTypeName];
                        instanceFactory.SetDefault(defaultInstance.DefaultKey);
                    }
                    else
                    {
                        setGenericTypeDefaults(defaultInstance, genericFamily);
                    }
                }
                catch (Exception)
                {
                    if (_failOnException)
                    {
                        throw;
                    }
                }
            }
        }

        private void setGenericTypeDefaults(InstanceDefault defaultInstance, PluginFamily genericFamily)
        {
            genericFamily.DefaultInstanceKey = defaultInstance.DefaultKey;
            Type genericType = genericFamily.PluginType;

            foreach (KeyValuePair<Type, IInstanceFactory> pair in _factories)
            {
                if (pair.Key.IsGenericType)
                {
                    if (pair.Key.GetGenericTypeDefinition().Equals(genericType))
                    {
                        pair.Value.SetDefault(defaultInstance.DefaultKey);
                    }
                }
            }
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
        /// Creates the named instance of the PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public object CreateInstance(Type pluginType, string instanceKey)
        {
            IInstanceFactory instanceFactory = this[pluginType];
            return instanceFactory.GetInstance(instanceKey);
        }



        /// <summary>
        /// Creates a new object instance of the requested type
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public object CreateInstance(Type pluginType)
        {
            IInstanceFactory instanceFactory = this[pluginType];
            return instanceFactory.GetInstance();
        }

        /// <summary>
        /// Creates a new object instance of the requested type
        /// </summary>
        /// <param name="pluginTypeName">Fully qualified name of the CLR Type to create</param>
        /// <returns></returns>
        public object CreateInstance(string pluginTypeName)
        {
            IInstanceFactory instanceFactory = this[pluginTypeName];
            return instanceFactory.GetInstance();
        }

        /// <summary>
        /// Creates a new instance of the requested type using the InstanceMemento.  Mostly used from other
        /// classes to link children members
        /// </summary>
        /// <param name="pluginTypeName"></param>
        /// <param name="instanceMemento"></param>
        /// <returns></returns>
        public object CreateInstance(string pluginTypeName, InstanceMemento instanceMemento)
        {
            IInstanceFactory instanceFactory = this[pluginTypeName];
            return instanceFactory.GetInstance(instanceMemento);
        }


        /// <summary>
        /// Creates a new instance of the requested type using the InstanceMemento.  Mostly used from other
        /// classes to link children members
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceMemento"></param>
        /// <returns></returns>
        public object CreateInstance(Type pluginType, InstanceMemento instanceMemento)
        {
            IInstanceFactory instanceFactory = this[pluginType];
            return instanceFactory.GetInstance(instanceMemento);
        }

        /// <summary>
        /// Creates an array of object instances of the requested type
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceMementoes"></param>
        /// <returns></returns>
        public Array CreateInstanceArray(string pluginType, InstanceMemento[] instanceMementoes)
        {
            if (instanceMementoes == null)
            {
                throw new StructureMapException(205, pluginType, "UNKNOWN");
            }

            IInstanceFactory instanceFactory = this[pluginType];
            return instanceFactory.GetArray(instanceMementoes);
        }

        /// <summary>
        /// Sets the default instance for the PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceMemento"></param>
        public void SetDefault(Type pluginType, InstanceMemento instanceMemento)
        {
            IInstanceFactory instanceFactory = this[pluginType];
            instanceFactory.SetDefault(instanceMemento);
        }

        /// <summary>
        /// Sets the default instance for the PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        public void SetDefault(Type pluginType, string instanceKey)
        {
            IInstanceFactory instanceFactory = this[pluginType];
            instanceFactory.SetDefault(instanceKey);
        }


        /// <summary>
        /// Sets the default instance for the PluginType
        /// </summary>
        /// <param name="pluginTypeName"></param>
        /// <param name="instanceKey"></param>
        public void SetDefault(string pluginTypeName, string instanceKey)
        {
            IInstanceFactory instanceFactory = this[pluginTypeName];
            instanceFactory.SetDefault(instanceKey);
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
                throw new StructureMapException(230);
            }

            IInstanceFactory factory = getOrCreateFactory(type);
            return factory.GetInstance();
        }

        private IInstanceFactory getOrCreateFactory(Type type)
        {
            return getOrCreateFactory(type, delegate(Type t)
                                                {
                                                    PluginFamily family =
                                                        PluginFamily.CreateAutoFilledPluginFamily(t);
                                                    return new InstanceFactory(family, true);
                                                });
        }

        protected IInstanceFactory getOrCreateFactory(Type type, CreateFactoryDelegate createFactory)
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

            LiteralMemento memento = new LiteralMemento(stub);
            this[pluginType].SetDefault(memento);
        }

        public IList GetAllInstances(Type type)
        {
            return this[type].GetAllInstances();
        }

        public void AddInstance<T>(InstanceMemento memento)
        {
            IInstanceFactory factory = getOrCreateFactory(typeof (T), createFactory);
            factory.AddInstance(memento);
        }

        public void AddInstance<PLUGINTYPE, CONCRETETYPE>()
        {
            IInstanceFactory factory = getOrCreateFactory(typeof (PLUGINTYPE), createFactory);
            InstanceMemento memento = factory.AddType<CONCRETETYPE>();
            factory.AddInstance(memento);
        }

        private InstanceFactory createFactory(Type pluggedType)
        {
            PluginFamily family = new PluginFamily(pluggedType);
            return new InstanceFactory(family, true);
        }

        public void AddDefaultInstance<PLUGINTYPE, CONCRETETYPE>()
        {
            IInstanceFactory factory = getOrCreateFactory(typeof (PLUGINTYPE), createFactory);
            InstanceMemento memento = factory.AddType<CONCRETETYPE>();
            factory.SetDefault(memento);
        }

        #region Nested type: CreateFactoryDelegate

        protected delegate InstanceFactory CreateFactoryDelegate(Type type);

        #endregion

        public string WhatDoIHave()
        {
            StringBuilder sb = new StringBuilder();

            foreach (IInstanceFactory factory in this)
            {
                sb.AppendFormat("PluginType {0}, Default: {1}\r\n", factory.PluginType.AssemblyQualifiedName,
                                factory.DefaultInstanceKey);
            }

            return sb.ToString();
        }
    }
}