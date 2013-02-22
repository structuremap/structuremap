using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using StructureMap.Pipeline;

namespace StructureMap
{
    /// <summary>
    /// A convenience "Containment" to hold your container if you are planning to have a single static <see cref="IContainer"/> instance in your application.
    /// 
    /// </summary>
    public static class ObjectFactory
    {
        private static readonly object _lockObject = new object();
        private static Lazy<Container> _containerBuilder = new Lazy<Container>(defaultContainer,LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// The Container that is kept alive by the ObjectFactory
        /// </summary>
        public static IContainer Container { get { return _containerBuilder.Value; } }

        private static Container defaultContainer()
        {
            var c = new Container();
            nameContainer(c);
            return c;
        }

        /// <summary>
        /// Fire up and initialize a new container. It is accessible through the <see cref="Container"/> property.
        /// Some convenience methods are available that route to this container. Passing no action equates to starting the container
        /// without any configuration. A subsequent call to this method will overwrite the reference that the Objectfactory held to the previous
        /// <see cref="IContainer"/> instance.
        /// </summary>
        public static void Initialize(Action<IInitializationExpression> action = null)
        {
            if (action == null)
                return;
            lock (_lockObject)
            {
                var expression = new InitializationExpression();
                action(expression);

                var graph = expression.BuildGraph();
                var container = new Container(graph);
                _containerBuilder = new Lazy<Container>(()=>container);
                nameContainer(container);
            }
        }

        /// <summary>
        /// Used to add additional configuration to a Container *after* the initialization.
        /// </summary>
        public static void Configure(Action<ConfigurationExpression> configure)
        {
            Container.Configure(configure);
        }

        /// <summary>
        /// Creates or finds the default instance of the pluginType
        /// </summary>
        public static object GetInstance(Type pluginType)
        {
            return Container.GetInstance(pluginType);
        }

        /// <summary>
        /// Creates or finds the default instance of type T
        /// </summary>
        public static TPluginType GetInstance<TPluginType>()
        {
            return Container.GetInstance<TPluginType>();
        }

        /// <summary>
        /// Creates or finds the named instance of the pluginType
        /// </summary>
        public static object GetNamedInstance(Type pluginType, string name)
        {
            return Container.GetInstance(pluginType, name);
        }

        /// <summary>
        /// Creates or finds the named instance of T
        /// </summary>
        public static TPluginType GetNamedInstance<TPluginType>(string name)
        {
            return Container.GetInstance<TPluginType>(name);
        }


        /// <summary>
        /// Creates or resolves all registered instances of the pluginType
        /// </summary>
        public static IEnumerable GetAllInstances(Type pluginType)
        {
            return Container.GetAllInstances(pluginType);
        }

        /// <summary>
        /// Creates or resolves all registered instances of type T
        /// </summary>
        public static IEnumerable<TPluginType> GetAllInstances<TPluginType>()
        {
            return Container.GetAllInstances<TPluginType>();
        }


        /// <summary>
        /// The "BuildUp" method takes in an already constructed object
        /// and uses Setter Injection to push in configured dependencies
        /// of that object.
        /// </summary>
        public static void BuildUp(object target)
        {
            Container.BuildUp(target);
        }

        private static void nameContainer(IContainer container)
        {
            container.Name = "ObjectFactory-" + container.Name; 
        }
    }
}