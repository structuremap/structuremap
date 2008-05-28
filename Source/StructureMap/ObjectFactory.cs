using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
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
        /// Attempts to create a new instance of the requested type.  Automatically inserts the default
        /// configured instance for each dependency in the StructureMap constructor function.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object FillDependencies(Type type)
        {
            return manager.FillDependencies(type);
        }

        public static T FillDependencies<T>()
        {
            return (T) manager.FillDependencies(typeof (T));
        }

        /// <summary>
        /// Sets up StructureMap to return the object in the "stub" argument anytime
        /// any instance of the PluginType is requested
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="stub"></param>
        public static void InjectStub(Type pluginType, object stub)
        {
            manager.InjectStub(pluginType, stub);
        }

        /// <summary>
        /// Sets up StructureMap to return the object in the "stub" argument anytime
        /// any instance of the PluginType is requested
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="stub"></param>
        public static void InjectStub<PLUGINTYPE>(PLUGINTYPE stub)
        {
            manager.InjectStub(typeof (PLUGINTYPE), stub);
        }


        public static string WhatDoIHave()
        {
            return manager.WhatDoIHave();
        }

        /// <summary>
        /// Sets the default instance of PLUGINTYPE to the object in the instance argument
        /// </summary
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="instance"></param>
        public static void Inject<PLUGINTYPE>(PLUGINTYPE instance)
        {
            manager.Inject<PLUGINTYPE>(instance);
        }

        /// <summary>
        /// Injects a new instance of PLUGINTYPE by name.  
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="instance"></param>
        /// <param name="instanceKey"></param>
        public static void InjectByName<PLUGINTYPE>(PLUGINTYPE instance, string instanceKey)
        {
            manager.InjectByName<PLUGINTYPE>(instance, instanceKey);
        }

        /// <summary>
        /// Injects a new instance of CONCRETETYPE to PLUGINTYPE by name.  
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <typeparam name="CONCRETETYPE"></typeparam>
        /// <param name="instanceKey"></param>
        public static void InjectByName<PLUGINTYPE, CONCRETETYPE>(string instanceKey)
        {
            manager.InjectByName<PLUGINTYPE, CONCRETETYPE>(instanceKey);
        }

        /// <summary>
        /// StructureMap will return an instance of CONCRETETYPE whenever
        /// a PLUGINTYPE is requested
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <typeparam name="CONCRETETYPE"></typeparam>
        public static void InjectDefaultType<PLUGINTYPE, CONCRETETYPE>() where CONCRETETYPE : PLUGINTYPE
        {
            manager.AddDefaultInstance<PLUGINTYPE, CONCRETETYPE>();
        }

        /// <summary>
        /// Adds a new CONCRETETYPE to StructureMap so that an instance of CONCRETETYPE
        /// will be returned from a call to ObjectFactory.GetAllInstance&lt;PLUGINTYPE&gt;()
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <typeparam name="CONCRETETYPE"></typeparam>
        public static void AddType<PLUGINTYPE, CONCRETETYPE>() where CONCRETETYPE : PLUGINTYPE
        {
            manager.AddInstance<PLUGINTYPE, CONCRETETYPE>();
        }

        #region InstanceManager and setting defaults

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

        /// <summary>
        /// Gets or sets the current named profile.  When set, overrides the default object instances
        /// according to the configured profile in StructureMap.config
        /// </summary>
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

        /// <summary>
        /// Restarts ObjectFactory.  Use with caution.
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
        /// Strictly used for testing scenarios
        /// </summary>
        /// <param name="manager"></param>
        internal static void ReplaceManager(IContainer manager)
        {
            _manager = manager;
        }


        /// <summary>
        /// Fires when the ObjectFactory is refreshed
        /// </summary>
        public static event Notify Refresh
        {
            add { _notify += value; }
            remove { _notify -= value; }
        }


        /// <summary>
        /// Restores all default instance settings according to the StructureMap.config files
        /// </summary>
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


        private static InstanceManager buildManager()
        {
            PluginGraph graph = StructureMapConfiguration.GetPluginGraph();

            InstanceManager instanceManager = new InstanceManager(graph);
            instanceManager.SetDefaultsToProfile(_profile);

            return instanceManager;
        }

        #endregion


        /// <summary>
        /// Returns the default instance of the requested System.Type
        /// </summary>
        /// <param name="TargetType"></param>
        /// <returns></returns>
        public static object GetInstance(Type TargetType)
        {
            return manager.GetInstance(TargetType);
        }

        /// <summary>
        /// Returns the default instance of the requested System.Type
        /// </summary>
        /// <typeparam name="TargetType"></typeparam>
        /// <returns></returns>
        public static TargetType GetInstance<TargetType>()
        {
            return (TargetType) manager.GetInstance(typeof (TargetType));
        }

        /// <summary>
        /// Builds an instance of the TargetType for the given InstanceMemento
        /// </summary>
        /// <param name="TargetType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static object GetInstance(Type TargetType, Instance instance)
        {
            return manager.GetInstance(TargetType, instance);
        }

        /// <summary>
        /// Builds an instance of the TargetType for the given InstanceMemento
        /// </summary>
        /// <typeparam name="TargetType"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static TargetType GetInstance<TargetType>(Instance instance)
        {
            return (TargetType) manager.GetInstance(typeof (TargetType), instance);
        }

        /// <summary>
        /// Returns the named instance of the requested System.Type
        /// </summary>
        /// <param name="TargetType"></param>
        /// <param name="InstanceName"></param>
        /// <returns></returns>
        public static object GetNamedInstance(Type TargetType, string InstanceName)
        {
            return manager.GetInstance(TargetType, InstanceName);
        }

        /// <summary>
        /// Returns the named instance of the requested System.Type
        /// </summary>
        /// <typeparam name="TargetType"></typeparam>
        /// <param name="InstanceName"></param>
        /// <returns></returns>
        public static TargetType GetNamedInstance<TargetType>(string InstanceName)
        {
            return (TargetType) manager.GetInstance(typeof (TargetType), InstanceName);
        }

        /// <summary>
        /// Sets the default instance of the TargetType
        /// </summary>
        /// <param name="TargetType"></param>
        /// <param name="InstanceName"></param>
        public static void SetDefaultInstanceName(Type TargetType, string InstanceName)
        {
            manager.SetDefault(TargetType, InstanceName);
        }

        /// <summary>
        /// Sets the default instance of the TargetType
        /// </summary>
        /// <typeparam name="TargetType"></typeparam>
        /// <param name="InstanceName"></param>
        public static void SetDefaultInstanceName<TargetType>(string InstanceName)
        {
            manager.SetDefault(typeof (TargetType), InstanceName);
        }


        /// <summary>
        /// Retrieves a list of all of the configured instances for a particular type
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static IList GetAllInstances(Type targetType)
        {
            return manager.GetAllInstances(targetType);
        }

        /// <summary>
        /// Retrieves a list of all of the configured instances for a particular type
        /// </summary>
        /// <typeparam name="TargetType"></typeparam>
        /// <returns></returns>
        public static IList<TargetType> GetAllInstances<TargetType>()
        {
            return manager.GetAllInstances<TargetType>();
        }

        public static ExplicitArgsExpression With<T>(T arg)
        {
            return manager.With(arg);
        }

        public static IExplicitProperty With(string argName)
        {
            return manager.With(argName);
        }


    }


}