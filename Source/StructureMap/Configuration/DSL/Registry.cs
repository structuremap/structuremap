using System;
using System.Collections.Generic;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Interceptors;

namespace StructureMap.Configuration.DSL
{
    public class Registry : IDisposable
    {
        private readonly List<IExpression> _expressions = new List<IExpression>();
        private readonly PluginGraph _graph;

        public Registry(PluginGraph graph) : this()
        {
            _graph = graph;
        }

        public Registry()
        {
            _graph = new PluginGraph();
            configure();
        }

        #region IDisposable Members

        public void Dispose()
        {
            ConfigurePluginGraph(_graph);
        }

        #endregion

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

        internal void addExpression(PluginGraphAlteration alteration)
        {
            _expressions.Add(new BasicExpression(alteration));
        }

        internal void ConfigurePluginGraph(PluginGraph graph)
        {
            foreach (IExpression expression in _expressions)
            {
                expression.Configure(graph);
            }
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

        public void RegisterInterceptor(TypeInterceptor interceptor)
        {
            addExpression(
                delegate(PluginGraph pluginGraph) { pluginGraph.InterceptorLibrary.AddInterceptor(interceptor); });
        }

        public TypeInterceptorExpression IfTypeMatches(Predicate<Type> match)
        {
            TypeInterceptorExpression expression = new TypeInterceptorExpression(match);
            _expressions.Add(expression);

            return expression;
        }


        /// <summary>
        /// Programmatically determine Assembly's to be scanned for attribute configuration
        /// </summary>
        /// <returns></returns>
        public ScanAssembliesExpression ScanAssemblies()
        {
            ScanAssembliesExpression expression = new ScanAssembliesExpression(this);
            addExpression(expression);

            return expression;
        }
    }


    public class TypeInterceptorExpression : IExpression, TypeInterceptor
    {
        private readonly Predicate<Type> _match;
        private InterceptionDelegate _interception;

        internal TypeInterceptorExpression(Predicate<Type> match)
        {
            _match = match;
        }

        #region IExpression Members

        void IExpression.Configure(PluginGraph graph)
        {
            graph.InterceptorLibrary.AddInterceptor(this);
        }

        #endregion

        #region TypeInterceptor Members

        public bool MatchesType(Type type)
        {
            return _match(type);
        }

        public object Process(object target)
        {
            return _interception(target);
        }

        #endregion

        public void InterceptWith(InterceptionDelegate interception)
        {
            _interception = interception;
        }
    }

    internal delegate void PluginGraphAlteration(PluginGraph pluginGraph);

    internal class BasicExpression : IExpression
    {
        private readonly PluginGraphAlteration _alteration;

        internal BasicExpression(PluginGraphAlteration alteration)
        {
            _alteration = alteration;
        }

        #region IExpression Members

        public void Configure(PluginGraph graph)
        {
            _alteration(graph);
        }

        #endregion
    }
}