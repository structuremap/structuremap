using System;
using System.Collections.Generic;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public class Registry : IDisposable
    {
        private List<IExpression> _expressions = new List<IExpression>();
        private PluginGraph _graph;

        public Registry(PluginGraph graph) : this()
        {
            _graph = graph;
        }

        public Registry()
        {
            _graph = new PluginGraph();
            configure();
        }


        /// <summary>
        /// Implement this method to 
        /// </summary>
        protected virtual void configure()
        {
            // no-op;
        }

        protected internal void addExpression(IExpression expression)
        {
            _expressions.Add(expression);
        }

        internal void ConfigurePluginGraph(PluginGraph graph)
        {
            foreach (IExpression expression in _expressions)
            {
                expression.Configure(graph);
            }
        }


        public void Dispose()
        {
            ConfigurePluginGraph(_graph);
        }

        /// <summary>
        /// Direct StructureMap to build instances of type T, and look for concrete classes
        /// marked with the [Pluggable] attribute that implement type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CreatePluginFamilyExpression BuildInstancesOf<T>()
        {
            CreatePluginFamilyExpression expression = new CreatePluginFamilyExpression(typeof (T));
            addExpression(expression);

            return expression;
        }

        public IInstanceManager BuildInstanceManager()
        {
            ConfigurePluginGraph(_graph);
            _graph.ReadDefaults();
            return new InstanceManager(_graph);
        }

        /// <summary>
        /// Starts an instance definition of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public InstanceExpression.InstanceTypeExpression AddInstanceOf<T>()
        {
            InstanceExpression expression = new InstanceExpression(typeof (T));
            addExpression(expression);
            return expression.TypeExpression();
        }

        /// <summary>
        /// Convenience method to start the definition of an instance of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static InstanceExpression.InstanceTypeExpression Instance<T>()
        {
            InstanceExpression expression = new InstanceExpression(typeof (T));
            return expression.TypeExpression();
        }

        /// <summary>
        /// Convenience method to register a prototype instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prototype"></param>
        /// <returns></returns>
        public static PrototypeExpression<T> Prototype<T>(T prototype)
        {
            return new PrototypeExpression<T>(prototype);
        }

        /// <summary>
        /// Convenience method to register a preconfigured instance of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static LiteralExpression<T> Object<T>(T instance)
        {
            return new LiteralExpression<T>(instance);
        }

        /// <summary>
        /// Registers a preconfigured instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public LiteralExpression<T> AddInstanceOf<T>(T target)
        {
            LiteralExpression<T> literal = new LiteralExpression<T>(target);
            addExpression(literal);

            return literal;
        }

        /// <summary>
        /// Add a preconfigured instance as a Prototype
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prototype"></param>
        /// <returns></returns>
        public PrototypeExpression<T> AddPrototypeInstanceOf<T>(T prototype)
        {
            PrototypeExpression<T> expression = new PrototypeExpression<T>(prototype);
            addExpression(expression);

            return expression;
        }

        /// <summary>
        /// convenience method for a UserControl
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static UserControlExpression LoadUserControlFrom<T>(string url)
        {
            return new UserControlExpression(typeof (T), url);
        }

        /// <summary>
        /// Starts the definition of a new Profile
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public ProfileExpression CreateProfile(string profileName)
        {
            ProfileExpression expression = new ProfileExpression(profileName);
            addExpression(expression);

            return expression;
        }

        public static bool IsPublicRegistry(Type type)
        {
            if (!typeof (Registry).IsAssignableFrom(type))
            {
                return false;
            }

            if (type.IsInterface || type.IsAbstract || type.IsGenericType)
            {
                return false;
            }

            return (type.GetConstructor(new Type[0]) != null);
        }

        /// <summary>
        /// Registers a UserControl as an instance 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public UserControlExpression LoadControlFromUrl<T>(string url)
        {
            UserControlExpression expression = new UserControlExpression(typeof (T), url);
            addExpression(expression);

            return expression;
        }
    }
}