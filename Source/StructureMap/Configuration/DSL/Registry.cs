using System;
using System.Collections.Generic;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL
{
    public delegate object InterceptionDelegate(object target);

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
            return new InstanceManager(_graph);
        }

        /// <summary>
        /// Starts an instance definition of type T
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        public ConfiguredInstance AddInstanceOf<PLUGINTYPE>()
        {
            ConfiguredInstance instance = new ConfiguredInstance();

            addExpression(delegate (PluginGraph pluginGraph)
                              {
                                  pluginGraph.LocateOrCreateFamilyForType(typeof(PLUGINTYPE)).AddInstance(instance);
                              });

            return instance;
        }


        /// <summary>
        /// Convenience method to start the definition of an instance of type T
        /// </summary>
        /// <typeparam name="PLUGGEDTYPE"></typeparam>
        /// <returns></returns>
        public static ConfiguredInstance Instance<PLUGGEDTYPE>()
        {
            ConfiguredInstance instance = new ConfiguredInstance();
            instance.PluggedType = typeof (PLUGGEDTYPE);

            return instance;
        }

        /// <summary>
        /// Convenience method to register a prototype instance
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="prototype"></param>
        /// <returns></returns>
        public static PrototypeInstance Prototype<PLUGINTYPE>(PLUGINTYPE prototype)
        {
            return new PrototypeInstance((ICloneable) prototype);
        }

        /// <summary>
        /// Convenience method to register a preconfigured instance of type T
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static LiteralInstance Object<PLUGINTYPE>(PLUGINTYPE instance)
        {
            return new LiteralInstance(instance);
        }

        /// <summary>
        /// Registers a preconfigured instance
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public LiteralInstance AddInstanceOf<PLUGINTYPE>(PLUGINTYPE target)
        {
            LiteralInstance literal = new LiteralInstance(target);
            _graph.LocateOrCreateFamilyForType(typeof(PLUGINTYPE)).AddInstance(literal);

            return literal;
        }

        /// <summary>
        /// Add a preconfigured instance as a Prototype
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="prototype"></param>
        /// <returns></returns>
        public PrototypeInstance AddPrototypeInstanceOf<PLUGINTYPE>(PLUGINTYPE prototype)
        {
            PrototypeInstance expression = new PrototypeInstance((ICloneable) prototype);
            _graph.LocateOrCreateFamilyForType(typeof(PLUGINTYPE)).AddInstance(expression);

            return expression;
        }

        /// <summary>
        /// convenience method for a UserControl
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static UserControlInstance LoadUserControlFrom<PLUGINTYPE>(string url)
        {
            return new UserControlInstance(url);
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
        public UserControlInstance LoadControlFromUrl<PLUGINTYPE>(string url)
        {
            UserControlInstance instance = new UserControlInstance(url);

            PluginFamily family = _graph.LocateOrCreateFamilyForType(typeof (PLUGINTYPE));
            family.AddInstance(instance);
            
            return instance;
        }

        public static ConstructorInstance ConstructedBy<PLUGINTYPE>
            (BuildObjectDelegate builder)
        {
            return new ConstructorInstance(builder);
        }

        public static ReferencedInstance Instance(string referencedKey)
        {
            return new ReferencedInstance(referencedKey);
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