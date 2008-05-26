using System;
using System.Collections.Generic;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL
{
    public class Registry : IDisposable
    {
        private readonly List<Action<PluginGraph>> _actions = new List<Action<PluginGraph>>();
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

        internal void addExpression(Action<PluginGraph> alteration)
        {
            _actions.Add(alteration);
        }

        internal void ConfigurePluginGraph(PluginGraph graph)
        {
            graph.Log.StartSource("Registry:  " + TypePath.GetAssemblyQualifiedName(GetType()));

            foreach (Action<PluginGraph> action in _actions)
            {
                action(graph);
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
            return new CreatePluginFamilyExpression<PLUGINTYPE>(this);
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
            return new CreatePluginFamilyExpression<PLUGINTYPE>(this);
        }

        public IInstanceManager BuildInstanceManager()
        {
            ConfigurePluginGraph(_graph);
            return new InstanceManager(_graph);
        }

        public PluginGraph Build()
        {
            ConfigurePluginGraph(_graph);
            _graph.Seal();

            return _graph;
        }

        /// <summary>
        /// Starts an instance definition of type T
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        public ConfiguredInstance AddInstanceOf<PLUGINTYPE>()
        {
            ConfiguredInstance instance = new ConfiguredInstance();

            addExpression(
                delegate(PluginGraph pluginGraph)
                {
                    pluginGraph.FindFamily(typeof (PLUGINTYPE)).AddInstance(instance);
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
            _graph.FindFamily(typeof (PLUGINTYPE)).AddInstance(literal);

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
            _graph.FindFamily(typeof (PLUGINTYPE)).AddInstance(expression);

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
            ProfileExpression expression = new ProfileExpression(profileName, this);

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

            PluginFamily family = _graph.FindFamily(typeof (PLUGINTYPE));
            family.AddInstance(instance);

            return instance;
        }

        public static ConstructorInstance ConstructedBy<PLUGINTYPE>
            (Func<PLUGINTYPE> builder)
        {
            return new ConstructorInstance(delegate() { return builder(); });
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

        public MatchedTypeInterceptor IfTypeMatches(Predicate<Type> match)
        {
            MatchedTypeInterceptor interceptor = new MatchedTypeInterceptor(match);
            _actions.Add(delegate(PluginGraph graph) { graph.InterceptorLibrary.AddInterceptor(interceptor); });

            return interceptor;
        }


        /// <summary>
        /// Programmatically determine Assembly's to be scanned for attribute configuration
        /// </summary>
        /// <returns></returns>
        public ScanAssembliesExpression ScanAssemblies()
        {
            return new ScanAssembliesExpression(this);
        }


        public void AddInstanceOf(Type pluginType, Instance instance)
        {
            _actions.Add(delegate(PluginGraph graph) { graph.FindFamily(pluginType).AddInstance(instance); });
        }

        public void AddInstanceOf<PLUGINTYPE>(Instance instance)
        {
            _actions.Add(delegate(PluginGraph graph) { graph.FindFamily(typeof (PLUGINTYPE)).AddInstance(instance); });
        }
    }


    public class MatchedTypeInterceptor : TypeInterceptor
    {
        private readonly Predicate<Type> _match;
        private InterceptionFunction _interception;

        internal MatchedTypeInterceptor(Predicate<Type> match)
        {
            _match = match;
        }

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

        public void InterceptWith(InterceptionFunction interception)
        {
            _interception = interception;
        }
    }
}