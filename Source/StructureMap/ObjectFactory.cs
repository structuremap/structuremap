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
    [EnvironmentPermission(SecurityAction.Assert, Read = "COMPUTERNAME")]
    public class ObjectFactory
    {
        private static readonly object _lockObject = new object();
        private static IContainer _container;
        private static string _profile = string.Empty;

        public static IModel Model
        {
            get { return container.Model; }
        }

        private static event Notify _notify;

        /// <summary>
        /// Restarts ObjectFactory and blows away all Singleton's and cached instances.  Use with caution.
        /// </summary>
        internal static void Reset()
        {
            lock (_lockObject)
            {
                StructureMapConfiguration.Unseal();

                _container = null;
                _profile = string.Empty;

                if (_notify != null)
                {
                    _notify();
                }
            }
        }

        public static void Initialize(Action<InitializationExpression> action)
        {
            lock (typeof (ObjectFactory))
            {
                var expression = new InitializationExpression();
                action(expression);

                PluginGraph graph = expression.BuildGraph();
                StructureMapConfiguration.Seal();

                _container = new Container(graph);
                Profile = expression.DefaultProfileName;
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
            return container.FillDependencies(type);
        }

        /// <summary>
        /// Creates an instance of the concrete type specified.  Dependencies are inferred from the constructor function of the type
        /// and automatically "filled"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FillDependencies<T>()
        {
            return (T) container.FillDependencies(typeof (T));
        }

        [Obsolete("Please use Inject(Type, object) instead.")]
        public static void InjectStub(Type pluginType, object stub)
        {
            Inject(pluginType, stub);
        }

        public static void Inject(Type pluginType, object instance)
        {
            container.Inject(pluginType, instance);
        }

        [Obsolete("Please use Inject() instead.")]
        public static void InjectStub<PLUGINTYPE>(PLUGINTYPE stub)
        {
            Inject(stub);
        }

        public static void Inject<PLUGINTYPE>(PLUGINTYPE instance)
        {
            container.Inject(instance);
        }

        public static void Inject<PLUGINTYPE>(string name, PLUGINTYPE instance)
        {
            container.Inject(name, instance);
        }

        [Obsolete("Please use Inject<PLUGINTYPE>(name) instead.")]
        public static void InjectStub<PLUGINTYPE>(string name, PLUGINTYPE stub)
        {
            Inject(name, stub);
        }


        public static string WhatDoIHave()
        {
            return container.WhatDoIHave();
        }

        public static void AssertConfigurationIsValid()
        {
            container.AssertConfigurationIsValid();
        }

        /// <summary>
        /// Returns and/or constructs the default instance of the requested System.Type
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public static object GetInstance(Type pluginType)
        {
            return container.GetInstance(pluginType);
        }

        /// <summary>
        /// Returns and/or constructs the default instance of the requested System.Type
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        public static PLUGINTYPE GetInstance<PLUGINTYPE>()
        {
            return (PLUGINTYPE) container.GetInstance(typeof (PLUGINTYPE));
        }

        public static object GetInstance(Type TargetType, Instance instance)
        {
            return container.GetInstance(TargetType, instance);
        }

        public static TargetType GetInstance<TargetType>(Instance instance)
        {
            return (TargetType) container.GetInstance(typeof (TargetType), instance);
        }

        /// <summary>
        /// Retrieves an instance of pluginType by name
        /// </summary>
        /// <param name="pluginType">The PluginType</param>
        /// <param name="name">The instance name</param>
        /// <returns></returns>
        public static object GetNamedInstance(Type pluginType, string name)
        {
            return container.GetInstance(pluginType, name);
        }

        /// <summary>
        /// Retrieves an instance of PLUGINTYPE by name
        /// </summary>
        /// <typeparam name="PLUGINTYPE">The PluginType</typeparam>
        /// <param name="name">The instance name</param>
        /// <returns></returns>
        public static PLUGINTYPE GetNamedInstance<PLUGINTYPE>(string name)
        {
            return (PLUGINTYPE) container.GetInstance(typeof (PLUGINTYPE), name);
        }

        public static void SetDefaultInstanceName(Type TargetType, string InstanceName)
        {
            container.SetDefault(TargetType, InstanceName);
        }

        public static void SetDefaultInstanceName<TargetType>(string InstanceName)
        {
            container.SetDefault(typeof (TargetType), InstanceName);
        }

        /// <summary>
        /// Retrieves all instances of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public static IList GetAllInstances(Type pluginType)
        {
            return container.GetAllInstances(pluginType);
        }

        /// <summary>
        /// Retrieves all instances of the PLUGINTYPE
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        public static IList<PLUGINTYPE> GetAllInstances<PLUGINTYPE>()
        {
            return container.GetAllInstances<PLUGINTYPE>();
        }

        /// <summary>
        /// Pass in an explicit argument of Type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static ExplicitArgsExpression With<T>(T arg)
        {
            return container.With(arg);
        }

        /// <summary>
        /// Pass in an explicit argument by name
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public static IExplicitProperty With(string argName)
        {
            return container.With(argName);
        }

        public static void EjectAllInstancesOf<T>()
        {
            container.EjectAllInstancesOf<T>();
        }

        #region Container and setting defaults

        private static IContainer container
        {
            get
            {
                if (_container == null)
                {
                    lock (_lockObject)
                    {
                        if (_container == null)
                        {
                            _container = buildManager();
                        }
                    }
                }

                return _container;
            }
        }


        public static string Profile
        {
            set
            {
                lock (_lockObject)
                {
                    _profile = value;
                    container.SetDefaultsToProfile(_profile);
                }
            }
            get { return _profile; }
        }

        internal static PluginGraph PluginGraph
        {
            get { return container.PluginGraph; }
        }


        internal static void ReplaceManager(IContainer container)
        {
            _container = container;
        }

        public static void Configure(Action<ConfigurationExpression> configure)
        {
            container.Configure(configure);
        }

        public static void SetDefault(Type pluginType, Instance instance)
        {
            container.SetDefault(pluginType, instance);
        }

        public static void SetDefault<PLUGINTYPE>(Instance instance)
        {
            container.SetDefault<PLUGINTYPE>(instance);
        }

        public static void SetDefault<PLUGINTYPE, CONCRETETYPE>() where CONCRETETYPE : PLUGINTYPE
        {
            container.SetDefault<PLUGINTYPE, CONCRETETYPE>();
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
            StructureMapConfiguration.Seal();

            var container = new Container(graph);
            container.SetDefaultsToProfile(_profile);

            return container;
        }

        #endregion
    }
}