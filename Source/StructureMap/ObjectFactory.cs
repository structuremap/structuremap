using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    public delegate void Notify();

    /// <summary>
    /// The main static Facade for the StructureMap container
    /// </summary>
    [EnvironmentPermission(SecurityAction.Assert, Read="COMPUTERNAME")]
    public class ObjectFactory
    {
        private static readonly object _lockObject = new object();
        private static IContainer _manager;
        private static string _profile = string.Empty;

        private static event Notify _notify;

        /// <summary>
        /// Used for testing only (kills singletons). In non-test scenarios, use Reset() instead.
        /// </summary>
        public static void ReInitialize()
        {
            _profile = string.Empty;
            _notify = null;
            _manager = null;
        }

        /// <summary>
        /// Restarts ObjectFactory and blows away all Singleton's and cached instances.  Use with caution.
        /// </summary>
        public static void Reset()
        {
            _manager = buildManager();

            if (_notify != null)
            {
                _notify();
            }
        }

        /// <summary>
        /// Creates an instance of the concrete type specified.  Dependencies are inferred from the constructor function of the type
        /// and automatically "filled"
        /// </summary>
        /// <param name="type">Must be a concrete type</param>
        /// <returns></returns>
        public static object FillDependencies(Type type)
        {
            return manager.FillDependencies(type);
        }

        /// <summary>
        /// Creates an instance of the concrete type specified.  Dependencies are inferred from the constructor function of the type
        /// and automatically "filled"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FillDependencies<T>()
        {
            return (T) manager.FillDependencies(typeof (T));
        }

        [Obsolete("Please use Inject(Type, object) instead.")]
        public static void InjectStub(Type pluginType, object stub)
        {
            Inject(pluginType, stub);
        }

        public static void Inject(Type pluginType, object instance)
        {
            manager.Inject(pluginType, instance);
        }

        [Obsolete("Please use Inject() instead.")]
        public static void InjectStub<PLUGINTYPE>(PLUGINTYPE stub)
        {
            Inject<PLUGINTYPE>(stub);
        }

        public static void Inject<PLUGINTYPE>(PLUGINTYPE instance)
        {
            manager.Inject<PLUGINTYPE>(instance);
        }

        public static void Inject<PLUGINTYPE>(string name, PLUGINTYPE instance)
        {
            manager.Inject<PLUGINTYPE>(name, instance);
        }

        [Obsolete("Please use Inject<PLUGINTYPE>(name) instead.")]
        public static void InjectStub<PLUGINTYPE>(string name, PLUGINTYPE stub)
        {
            Inject<PLUGINTYPE>(name, stub);
        }






        public static string WhatDoIHave()
        {
            return manager.WhatDoIHave();
        }

        public static void AssertConfigurationIsValid()
        {
            manager.AssertConfigurationIsValid();
        }


        #region Container and setting defaults

        private static IContainer manager
        {
            get
            {
                if (_manager == null)
                {
                    lock (_lockObject)
                    {
                        if (_manager == null)
                        {
                            Reset();
                        }
                    }
                }

                return _manager;
            }
        }


        public static string Profile
        {
            set
            {
                lock (_lockObject)
                {
                    _profile = value;
                    manager.SetDefaultsToProfile(_profile);
                }
            }
            get { return _profile; }
        }




        internal static void ReplaceManager(IContainer container)
        {
            _manager = container;
        }

        public static void Configure(Action<Registry> configure)
        {
            manager.Configure(configure);
        }

        public static void SetDefault(Type pluginType, Instance instance)
        {
            manager.SetDefault(pluginType, instance);
        }
        
        public static void SetDefault<PLUGINTYPE>(Instance instance)
        {
            manager.SetDefault<PLUGINTYPE>(instance);
        }

        public static void SetDefault<PLUGINTYPE, CONCRETETYPE>() where CONCRETETYPE : PLUGINTYPE
        {
            manager.SetDefault<PLUGINTYPE, CONCRETETYPE>();
        }


        public static event Notify Refresh
        {
            add { _notify += value; }
            remove { _notify -= value; }
        }


        public static void ResetDefaults()
        {
            try
            {
                lock (_lockObject)
                {
                    Profile = string.Empty;
                }
            }
            catch (TypeInitializationException ex)
            {
                if (ex.InnerException is StructureMapException)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw;
                }
            }
        }


        private static Container buildManager()
        {
            PluginGraph graph = StructureMapConfiguration.GetPluginGraph();

            Container container = new Container(graph);
            container.SetDefaultsToProfile(_profile);

            return container;
        }

        #endregion


        /// <summary>
        /// Returns and/or constructs the default instance of the requested System.Type
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public static object GetInstance(Type pluginType)
        {
            return manager.GetInstance(pluginType);
        }

        /// <summary>
        /// Returns and/or constructs the default instance of the requested System.Type
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        public static PLUGINTYPE GetInstance<PLUGINTYPE>()
        {
            return (PLUGINTYPE) manager.GetInstance(typeof (PLUGINTYPE));
        }

        public static object GetInstance(Type TargetType, Instance instance)
        {
            return manager.GetInstance(TargetType, instance);
        }

        public static TargetType GetInstance<TargetType>(Instance instance)
        {
            return (TargetType) manager.GetInstance(typeof (TargetType), instance);
        }

        /// <summary>
        /// Retrieves an instance of pluginType by name
        /// </summary>
        /// <param name="pluginType">The PluginType</param>
        /// <param name="name">The instance name</param>
        /// <returns></returns>
        public static object GetNamedInstance(Type pluginType, string name)
        {
            return manager.GetInstance(pluginType, name);
        }

        /// <summary>
        /// Retrieves an instance of PLUGINTYPE by name
        /// </summary>
        /// <typeparam name="PLUGINTYPE">The PluginType</typeparam>
        /// <param name="name">The instance name</param>
        /// <returns></returns>
        public static PLUGINTYPE GetNamedInstance<PLUGINTYPE>(string name)
        {
            return (PLUGINTYPE) manager.GetInstance(typeof (PLUGINTYPE), name);
        }

        public static void SetDefaultInstanceName(Type TargetType, string InstanceName)
        {
            manager.SetDefault(TargetType, InstanceName);
        }

        public static void SetDefaultInstanceName<TargetType>(string InstanceName)
        {
            manager.SetDefault(typeof (TargetType), InstanceName);
        }

        /// <summary>
        /// Retrieves all instances of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public static IList GetAllInstances(Type pluginType)
        {
            return manager.GetAllInstances(pluginType);
        }

        /// <summary>
        /// Retrieves all instances of the PLUGINTYPE
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        public static IList<PLUGINTYPE> GetAllInstances<PLUGINTYPE>()
        {
            return manager.GetAllInstances<PLUGINTYPE>();
        }

        /// <summary>
        /// Pass in an explicit argument of Type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static ExplicitArgsExpression With<T>(T arg)
        {
            return manager.With(arg);
        }

        /// <summary>
        /// Pass in an explicit argument by name
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public static IExplicitProperty With(string argName)
        {
            return manager.With(argName);
        }


    }


}