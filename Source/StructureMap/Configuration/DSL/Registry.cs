using System;
using System.Collections.Generic;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL
{
    public interface IRegistry
    {

        CreatePluginFamilyExpression<PLUGINTYPE> BuildInstancesOf<PLUGINTYPE>();

        GenericFamilyExpression ForRequestedType(Type pluginType);
        Registry.BuildWithExpression<T> ForConcreteType<T>();


        CreatePluginFamilyExpression<PLUGINTYPE> ForRequestedType<PLUGINTYPE>();

        PluginGraph Build();
        IsExpression<T> InstanceOf<T>();
        GenericIsExpression InstanceOf(Type pluginType);


        ProfileExpression CreateProfile(string profileName);

        void CreateProfile(string profileName, Action<ProfileExpression> action);
        void RegisterInterceptor(TypeInterceptor interceptor);
        MatchedTypeInterceptor IfTypeMatches(Predicate<Type> match);


        void Scan(Action<IAssemblyScanner> action);

        CreatePluginFamilyExpression<PLUGINTYPE> FillAllPropertiesOfType<PLUGINTYPE>();
    }

    public class Registry : IRegistry
    {
        private readonly List<Action<PluginGraph>> _actions = new List<Action<PluginGraph>>();
        private readonly List<Action> _basicActions = new List<Action>();

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

        protected void registerAction(Action action)
        {
            _basicActions.Add(action);
        }

        internal void addExpression(Action<PluginGraph> alteration)
        {
            _actions.Add(alteration);
        }

        internal void ConfigurePluginGraph(PluginGraph graph)
        {
            if (graph.Registries.Contains(this)) return;

            graph.Log.StartSource("Registry:  " + TypePath.GetAssemblyQualifiedName(GetType()));

            _basicActions.ForEach(action => action());
            _actions.ForEach(action => action(graph));

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

        public BuildWithExpression<T> ForConcreteType<T>()
        {
            SmartInstance<T> instance = ForRequestedType<T>().TheDefault.Is.OfConcreteType<T>();
            return new BuildWithExpression<T>(instance);
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
            var graph = new PluginGraph();
            ConfigurePluginGraph(graph);
            graph.Seal();

            return graph;
        }

        public IsExpression<T> InstanceOf<T>()
        {
            return new InstanceExpression<T>(instance =>
            {
                Action<PluginGraph> alteration = g => g.FindFamily(typeof (T)).AddInstance(instance);
                _actions.Add(alteration);
            });
        }

        public GenericIsExpression InstanceOf(Type pluginType)
        {
            return
                new GenericIsExpression(
                    instance => { _actions.Add(graph => { graph.FindFamily(pluginType).AddInstance(instance); }); });
        }

        /// <summary>
        /// Starts the definition of a new Profile
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public ProfileExpression CreateProfile(string profileName)
        {
            var expression = new ProfileExpression(profileName, this);

            return expression;
        }

        public void CreateProfile(string profileName, Action<ProfileExpression> action)
        {
            var expression = new ProfileExpression(profileName, this);
            action(expression);
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
            var interceptor = new MatchedTypeInterceptor(match);
            _actions.Add(graph => graph.InterceptorLibrary.AddInterceptor(interceptor));

            return interceptor;
        }


        /// <summary>
        /// Designates a policy for scanning assemblies to auto
        /// register types
        /// </summary>
        /// <returns></returns>
        public void Scan(Action<IAssemblyScanner> action)
        {
            var scanner = new AssemblyScanner();
            action(scanner);

            _actions.Add(graph => graph.AddScanner(scanner));
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
            PluginCache.AddFilledType(typeof (PLUGINTYPE));
            return ForRequestedType<PLUGINTYPE>();
        }

        #region Nested type: BuildWithExpression

        public class BuildWithExpression<T>
        {
            private readonly SmartInstance<T> _instance;

            public BuildWithExpression(SmartInstance<T> instance)
            {
                _instance = instance;
            }

            public SmartInstance<T> Configure
            {
                get { return _instance; }
            }
        }

        #endregion
    }
}