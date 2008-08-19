using System;
using System.Collections.Generic;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL
{
    public class Registry : RegistryExpressions
    {
        private readonly List<Action<PluginGraph>> _actions = new List<Action<PluginGraph>>();

        public Registry()
        {
            configure();
        }

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
            if (graph.Registries.Contains(this)) return;

            graph.Log.StartSource("Registry:  " + TypePath.GetAssemblyQualifiedName(GetType()));

            foreach (Action<PluginGraph> action in _actions)
            {
                action(graph);
            }

            graph.Registries.Add(this);
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


        public GenericFamilyExpression ForRequestedType(Type pluginType)
        {
            return new GenericFamilyExpression(pluginType, this);
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

        public PluginGraph Build()
        {
            PluginGraph graph = new PluginGraph();
            ConfigurePluginGraph(graph);
            graph.Seal();

            return graph;
        }

        /// <summary>
        /// Starts an instance definition of type T
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        [Obsolete("Like to go away")]
        public ConfiguredInstanceExpression<PLUGINTYPE> AddInstanceOf<PLUGINTYPE>()
        {
            return new ConfiguredInstanceExpression<PLUGINTYPE>(this);
        }

        [Obsolete("Like to get rid of this")]
        public class ConfiguredInstanceExpression<PLUGINTYPE>
        {
            private readonly Registry _registry;

            public ConfiguredInstanceExpression(Registry registry)
            {
                _registry = registry;
            }


            public ConfiguredInstance UsingConcreteType<T>()
            {
                ConfiguredInstance instance = new ConfiguredInstance(typeof(T));
                _registry.addExpression(graph => graph.FindFamily(typeof(PLUGINTYPE)).AddInstance(instance));

                return instance;
            }
        }


        /// <summary>
        /// Registers a preconfigured instance
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        [Obsolete("Like to get rid of this")]
        public LiteralInstance AddInstanceOf<PLUGINTYPE>(PLUGINTYPE target)
        {
            LiteralInstance literal = new LiteralInstance(target);
            _actions.Add(graph => graph.FindFamily(typeof (PLUGINTYPE)).AddInstance(literal));

            return literal;
        }

        /// <summary>
        /// Add a preconfigured instance as a Prototype
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="prototype"></param>
        /// <returns></returns>
        [Obsolete("Like to get rid of this")]
        public PrototypeInstance AddPrototypeInstanceOf<PLUGINTYPE>(PLUGINTYPE prototype)
        {
            PrototypeInstance instance = new PrototypeInstance((ICloneable) prototype);
            _actions.Add(graph => graph.FindFamily(typeof (PLUGINTYPE)).AddInstance(instance));

            return instance;
        }

        /// <summary>
        /// convenience method for a UserControl
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [Obsolete("Like to get rid of this")]
        public static UserControlInstance LoadUserControlFrom(string url)
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

        public void RegisterInterceptor(TypeInterceptor interceptor)
        {
            addExpression(pluginGraph => pluginGraph.InterceptorLibrary.AddInterceptor(interceptor));
        }

        public MatchedTypeInterceptor IfTypeMatches(Predicate<Type> match)
        {
            MatchedTypeInterceptor interceptor = new MatchedTypeInterceptor(match);
            _actions.Add(graph => graph.InterceptorLibrary.AddInterceptor(interceptor));

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

        [Obsolete("Like to get rid of this")]
        public void AddInstanceOf(Type pluginType, Instance instance)
        {
            _actions.Add(graph => graph.FindFamily(pluginType).AddInstance(instance));
        }

        [Obsolete("Like to get rid of this")]
        public void AddInstanceOf<PLUGINTYPE>(Instance instance)
        {
            _actions.Add(graph => graph.FindFamily(typeof (PLUGINTYPE)).AddInstance(instance));
        }

        public bool Equals(Registry obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return GetType().Equals(obj.GetType());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (obj is Registry) return false;


            if (obj.GetType() != typeof (Registry)) return false;
            return Equals((Registry) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> FillAllPropertiesOfType<PLUGINTYPE>()
        {
            PluginCache.AddFilledType(typeof(PLUGINTYPE));
            return ForRequestedType<PLUGINTYPE>();
        }
    }
}