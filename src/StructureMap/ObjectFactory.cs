using System;
using System.Collections;
using System.Collections.Generic;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Query;

namespace StructureMap
{
    public delegate void Notify();

    /// <summary>
    /// The main static Facade for the StructureMap container
    /// </summary>
    //[EnvironmentPermission(SecurityAction.Assert, Read = "COMPUTERNAME")]
    public static class ObjectFactory
    {
        private static readonly object _lockObject = new object();
        private static Container _container;
        private static string _profile = string.Empty;

        /// <summary>
        /// Provides queryable access to the configured PluginType's and Instances of the inner Container
        /// </summary>
        public static IModel Model { get { return container.Model; } }

        public static IContainer Container { get { return container; } }

        private static event Notify _notify;

        /// <summary>
        /// Restarts ObjectFactory and blows away all Singleton's and cached instances.  Use with caution.
        /// </summary>
        internal static void Reset()
        {
            lock (_lockObject)
            {
                _container = null;
                _profile = string.Empty;

                if (_notify != null)
                {
                    _notify();
                }
            }
        }

        /// <summary>
        /// Remove and dispose all objects scoped by HttpContext.  Call this method at the *end* of an Http request to clean up resources
        /// </summary>
        public static void ReleaseAndDisposeAllHttpScopedObjects()
        {
            HttpContextLifecycle.DisposeAndClearAll();
        }

        public static void Initialize(Action<IInitializationExpression> action)
        {
            lock (typeof (ObjectFactory))
            {
                var expression = new InitializationExpression();
                action(expression);

                PluginGraph graph = expression.BuildGraph();

                _container = new Container(graph);
                nameContainer(_container);
                Profile = expression.DefaultProfileName;
            }
        }


        /// <summary>
        /// Injects the given object into a Container as the default for the designated
        /// pluginType.  Mostly used for temporarily setting up return values of the Container
        /// to introduce mocks or stubs during automated testing scenarios
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        public static void Inject(Type pluginType, object instance)
        {
            container.Inject(pluginType, instance);
        }

        /// <summary>
        /// Injects the given object into a Container as the default for the designated
        /// TPluginType.  Mostly used for temporarily setting up return values of the Container
        /// to introduce mocks or stubs during automated testing scenarios
        /// </summary>
        /// <typeparam name="TPluginType"></typeparam>
        /// <param name="instance"></param>
        public static void Inject<TPluginType>(TPluginType instance)
        {
            container.Inject(instance);
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
        /// <typeparam name="TPluginType"></typeparam>
        /// <returns></returns>
        public static TPluginType GetInstance<TPluginType>()
        {
            return container.GetInstance<TPluginType>();
        }

        /// <summary>
        /// Creates a new instance of the requested type using the supplied Instance.  Mostly used internally
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static object GetInstance(Type targetType, Instance instance)
        {
            return container.GetInstance(targetType, instance);
        }

        /// <summary>
        /// Creates a new instance of the requested type T using the supplied Instance.  Mostly used internally
        /// </summary>
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
        /// <param name="name"></param>
        /// <returns></returns>
        public static object GetNamedInstance(Type pluginType, string name)
        {
            return container.GetInstance(pluginType, name);
        }

        /// <summary>
        /// Creates or finds the named instance of T
        /// </summary>
        /// <typeparam name="TPluginType"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static TPluginType GetNamedInstance<TPluginType>(string name)
        {
            return container.GetInstance<TPluginType>(name);
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
        /// <typeparam name="TPluginType"></typeparam>
        /// <returns></returns>
        public static IList<TPluginType> GetAllInstances<TPluginType>()
        {
            return container.GetAllInstances<TPluginType>();
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
        /// Starts a request for an instance or instances with explicitly configured arguments.  Specifies that any dependency
        /// of type T should be "arg"
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static ExplicitArgsExpression With(Type pluginType, object arg)
        {
            return container.With(pluginType, arg);
        }

        /// <summary>
        /// Removes all configured instances of type T from the Container.  Use with caution!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void EjectAllInstancesOf<T>()
        {
            container.EjectAllInstancesOf<T>();
        }

        public static T GetInstance<T>(ExplicitArguments args)
        {
            return container.GetInstance<T>(args);
        }

        /// <summary>
        /// Creates or finds the named instance of the pluginType. Returns null if the named instance is not known to the container.
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public static object TryGetInstance(Type pluginType, string instanceKey)
        {
            return container.TryGetInstance(pluginType, instanceKey);
        }

        /// <summary>
        /// Creates or finds the default instance of the pluginType. Returns null if the pluginType is not known to the container.
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public static object TryGetInstance(Type pluginType)
        {
            return container.TryGetInstance(pluginType);
        }

        /// <summary>
        /// Creates or finds the default instance of type T. Returns the default value of T if it is not known to the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T TryGetInstance<T>()
        {
            return container.TryGetInstance<T>();
        }

        /// <summary>
        /// Creates or finds the named instance of type T. Returns the default value of T if the named instance is not known to the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instanceKey"></param>
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
        /// IFlattener flattener1 = container.ForGenericType(typeof (IFlattener&lt;&gt;))
        ///     .WithParameters(typeof (Address)).GetInstanceAs&lt;IFlattener&gt;();
        /// </example>
        public static Container.OpenGenericTypeExpression ForGenericType(Type templateType)
        {
            return container.ForGenericType(templateType);
        }


        /// <summary>
        /// Shortcut syntax for using an object to find a service that handles
        /// that type of object by using an open generic type
        /// </summary>
        /// <example>
        /// IHandler handler = container.ForObject(shipment)
        ///                        .GetClosedTypeOf(typeof (IHandler<>))
        ///                        .As<IHandler>();
        /// </example>
        /// <param name="subject"></param>
        /// <returns></returns>
        public static CloseGenericTypeExpression ForObject(object subject)
        {
            return container.ForObject(subject);
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
                            _container = new Container();
                            nameContainer(_container);
                        }
                    }
                }

                return _container;
            }
        }

        /// <summary>
        /// Sets the default instance for all PluginType's to the designated Profile.
        /// </summary>
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

        internal static PluginGraph PluginGraph { get { return container.PluginGraph; } }


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

        public static event Notify Refresh { add { _notify += value; } remove { _notify -= value; } }


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

        #endregion

        private static void nameContainer(IContainer container)
        {
            container.Name = "ObjectFactory-" + container.Name; 
        }
    }
}