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
    public static class ObjectFactory
    {
        private static readonly object _lockObject = new object();
        private static Container _container;
        private static string _profile = string.Empty;

        /// <summary>
        /// Provides queryable access to the configured PluginType's and Instances of the inner Container
        /// </summary>
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

        public static void Initialize(Action<IInitializationExpression> action)
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


        [Obsolete("Please use GetInstance(Type) instead")]
        public static object FillDependencies(Type type)
        {
            return container.FillDependencies(type);
        }

        [Obsolete("Please use GetInstance<T>() instead")]
        public static T FillDependencies<T>()
        {
            return (T) container.FillDependencies(typeof (T));
        }

        [Obsolete("Please use Inject(Type, object) instead.")]
        public static void InjectStub(Type pluginType, object stub)
        {
            Inject(pluginType, stub);
        }

        /// <summary>
        /// Injects the given object into a Container as the default for the designated
        /// pluginType.  Mostly used for temporarily setting up return values of the Container
        /// to introduce mocks or stubs during automated testing scenarios
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="stub"></param>
        public static void Inject(Type pluginType, object instance)
        {
            container.Inject(pluginType, instance);
        }

        [Obsolete("Please use Inject() instead.")]
        public static void InjectStub<PLUGINTYPE>(PLUGINTYPE stub)
        {
            Inject(stub);
        }

        /// <summary>
        /// Injects the given object into a Container as the default for the designated
        /// PLUGINTYPE.  Mostly used for temporarily setting up return values of the Container
        /// to introduce mocks or stubs during automated testing scenarios
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="instance"></param>
        public static void Inject<PLUGINTYPE>(PLUGINTYPE instance)
        {
            container.Inject(instance);
        }

        /// <summary>
        /// Injects the given object into a Container by name for the designated
        /// pluginType.  Mostly used for temporarily setting up return values of the Container
        /// to introduce mocks or stubs during automated testing scenarios
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="instance"></param>
        public static void Inject<PLUGINTYPE>(string name, PLUGINTYPE instance)
        {
            container.Inject(name, instance);
        }

        [Obsolete("Please use Inject<PLUGINTYPE>(name) instead.")]
        public static void InjectStub<PLUGINTYPE>(string name, PLUGINTYPE stub)
        {
            Inject(name, stub);
        }

        /// <summary>
        /// Returns a report detailing the complete configuration of all PluginTypes and Instances
        /// </summary>
        /// <returns></returns>
        public static string WhatDoIHave()
        {
            return container.WhatDoIHave();
        }

        /// <summary>
        /// Use with caution!  Does a full environment test of the configuration of this container.  Will try to create every configured
        /// instance and afterward calls any methods marked with the [ValidationMethod] attribute
        /// </summary>
        public static void AssertConfigurationIsValid()
        {
            container.AssertConfigurationIsValid();
        }

        /// <summary>
        /// Creates or finds the default instance of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public static object GetInstance(Type pluginType)
        {
            return container.GetInstance(pluginType);
        }

        /// <summary>
        /// Creates or finds the default instance of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static PLUGINTYPE GetInstance<PLUGINTYPE>()
        {
            return container.GetInstance<PLUGINTYPE>();
        }

        /// <summary>
        /// Creates a new instance of the requested type using the supplied Instance.  Mostly used internally
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static object GetInstance(Type TargetType, Instance instance)
        {
            return container.GetInstance(TargetType, instance);
        }

        /// <summary>
        /// Creates a new instance of the requested type T using the supplied Instance.  Mostly used internally
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static T GetInstance<T>(Instance instance)
        {
            return container.GetInstance<T>(instance);
        }

        /// <summary>
        /// Creates or finds the named instance of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public static object GetNamedInstance(Type pluginType, string name)
        {
            return container.GetInstance(pluginType, name);
        }

        /// <summary>
        /// Creates or finds the named instance of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public static PLUGINTYPE GetNamedInstance<PLUGINTYPE>(string name)
        {
            return container.GetInstance<PLUGINTYPE>(name);
        }


        /// <summary>
        /// Creates or resolves all registered instances of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public static IList GetAllInstances(Type pluginType)
        {
            return container.GetAllInstances(pluginType);
        }

        /// <summary>
        /// Creates or resolves all registered instances of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IList<PLUGINTYPE> GetAllInstances<PLUGINTYPE>()
        {
            return container.GetAllInstances<PLUGINTYPE>();
        }

        /// <summary>
        /// Starts a request for an instance or instances with explicitly configured arguments.  Specifies that any dependency
        /// of type T should be "arg"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static ExplicitArgsExpression With<T>(T arg)
        {
            return container.With(arg);
        }

        /// <summary>
        /// Starts a request for an instance or instances with explicitly configured arguments.  Specifies that any dependency or primitive argument
        /// with the designated name should be the next value.
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public static IExplicitProperty With(string argName)
        {
            return container.With(argName);
        }

        /// <summary>
        /// Removes all configured instances of type T from the Container.  Use with caution!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void EjectAllInstancesOf<T>()
        {
            container.EjectAllInstancesOf<T>();
        }

        #region Container and setting defaults

        private static Container container
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

        /// <summary>
        /// Sets the default instance for all PluginType's to the designated Profile.
        /// </summary>
        /// <param name="profile"></param>
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


        internal static void ReplaceManager(Container container)
        {
            _container = container;
        }

        /// <summary>
        /// Used to add additional configuration to a Container *after* the initialization.
        /// </summary>
        /// <param name="configure"></param>
        public static void Configure(Action<ConfigurationExpression> configure)
        {
            container.Configure(configure);
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

        public static T GetInstance<T>(ExplicitArguments args)
        {
            return container.GetInstance<T>(args);
        }

        /// <summary>
        /// Creates or finds the named instance of the pluginType. Returns null if the named instance is not known to the container.
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static object TryGetInstance(Type pluginType, string instanceKey)
        {
            return container.TryGetInstance(pluginType, instanceKey);
        }

        /// <summary>
        /// Creates or finds the default instance of the pluginType. Returns null if the pluginType is not known to the container.
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static object TryGetInstance(Type pluginType)
        {
            return container.TryGetInstance(pluginType);
        }

        /// <summary>
        /// Creates or finds the default instance of type T. Returns the default value of T if it is not known to the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static T TryGetInstance<T>()
        {
            return container.TryGetInstance<T>();
        }

        /// <summary>
        /// Creates or finds the named instance of type T. Returns the default value of T if the named instance is not known to the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static T TryGetInstance<T>(string instanceKey)
        {
            return container.TryGetInstance<T>(instanceKey);
        }

        /// <summary>
        /// The "BuildUp" method takes in an already constructed object
        /// and uses Setter Injection to push in configured dependencies
        /// of that object
        /// </summary>
        /// <param name="target"></param>
        public static void BuildUp(object target)
        {
            container.BuildUp(target);
        }

        /// <summary>
        /// Convenience method to request an object using an Open Generic
        /// Type and its parameter Types
        /// </summary>
        /// <param name="templateType"></param>
        /// <returns></returns>
        /// <example>
        /// IFlattener flattener1 = container.ForGenericType(typeof (IFlattener<>))
        ///     .WithParameters(typeof (Address)).GetInstanceAs<IFlattener>();
        /// </example>
        public static Container.OpenGenericTypeExpression ForGenericType(Type templateType)
        {
            return container.ForGenericType(templateType);
        }

    }
}