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
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> BuildInstancesOf<PLUGINTYPE>()
        {
            CreatePluginFamilyExpression<PLUGINTYPE> expression = new CreatePluginFamilyExpression<PLUGINTYPE>();
            addExpression(expression);

            return expression;
        }

        /// <summary>
        /// Direct StructureMap to build instances of type T, and look for concrete classes
        /// marked with the [Pluggable] attribute that implement type T.
        /// 
        /// This is the equivalent of calling BuildInstancesOf<T>()
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> ForRequestedType<PLUGINTYPE>()
        {
            CreatePluginFamilyExpression<PLUGINTYPE> expression = new CreatePluginFamilyExpression<PLUGINTYPE>();
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
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        public InstanceExpression.InstanceTypeExpression AddInstanceOf<PLUGINTYPE>()
        {
            InstanceExpression expression = new InstanceExpression(typeof (PLUGINTYPE));
            addExpression(expression);
            return expression.TypeExpression();
        }

       

        /// <summary>
        /// Convenience method to start the definition of an instance of type T
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        public static InstanceExpression.InstanceTypeExpression Instance<PLUGINTYPE>()
        {
            InstanceExpression expression = new InstanceExpression(typeof (PLUGINTYPE));
            return expression.TypeExpression();
        }

        /// <summary>
        /// Convenience method to register a prototype instance
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="prototype"></param>
        /// <returns></returns>
        public static PrototypeExpression<PLUGINTYPE> Prototype<PLUGINTYPE>(PLUGINTYPE prototype)
        {
            return new PrototypeExpression<PLUGINTYPE>(prototype);
        }

        /// <summary>
        /// Convenience method to register a preconfigured instance of type T
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static LiteralExpression<PLUGINTYPE> Object<PLUGINTYPE>(PLUGINTYPE instance)
        {
            return new LiteralExpression<PLUGINTYPE>(instance);
        }

        /// <summary>
        /// Registers a preconfigured instance
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public LiteralExpression<PLUGINTYPE> AddInstanceOf<PLUGINTYPE>(PLUGINTYPE target)
        {
            LiteralExpression<PLUGINTYPE> literal = new LiteralExpression<PLUGINTYPE>(target);
            addExpression(literal);

            return literal;
        }

        /// <summary>
        /// Add a preconfigured instance as a Prototype
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="prototype"></param>
        /// <returns></returns>
        public PrototypeExpression<PLUGINTYPE> AddPrototypeInstanceOf<PLUGINTYPE>(PLUGINTYPE prototype)
        {
            PrototypeExpression<PLUGINTYPE> expression = new PrototypeExpression<PLUGINTYPE>(prototype);
            addExpression(expression);

            return expression;
        }

        /// <summary>
        /// convenience method for a UserControl
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static UserControlExpression LoadUserControlFrom<PLUGINTYPE>(string url)
        {
            return new UserControlExpression(typeof (PLUGINTYPE), url);
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
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public UserControlExpression LoadControlFromUrl<PLUGINTYPE>(string url)
        {
            UserControlExpression expression = new UserControlExpression(typeof (PLUGINTYPE), url);
            addExpression(expression);

            return expression;
        }

        public static ConstructorExpression<PLUGINTYPE> ConstructedBy<PLUGINTYPE>
            (BuildObjectDelegate<PLUGINTYPE> builder)
        {
            return new ConstructorExpression<PLUGINTYPE>(builder);
        }

        public static ReferenceMementoBuilder Instance(string referencedKey)
        {
            return new ReferenceMementoBuilder(referencedKey);
        }
        
    }
}