using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL
{
    public interface IRegistry
    {
        /// <summary>
        /// Adds the concreteType as an Instance of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="concreteType"></param>
        void AddType(Type pluginType, Type concreteType);

        /// <summary>
        /// Adds the concreteType as an Instance of the pluginType with a name
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="concreteType"></param>
        /// <param name="name"></param>
        void AddType(Type pluginType, Type concreteType, string name);

        /// <summary>
        /// Add the pluggedType as an instance to any configured pluginType where pluggedType
        /// could be assigned to the pluginType
        /// </summary>
        /// <param name="pluggedType"></param>
        void AddType(Type pluggedType);

        /// <summary>
        /// Imports the configuration from another registry into this registry.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void IncludeRegistry<T>() where T : Registry, new();

        /// <summary>
        /// Imports the configuration from another registry into this registry.
        /// </summary>
        /// <param name="registry"></param>
        void IncludeRegistry(Registry registry);

        /// <summary>
        /// Expression Builder used to define policies for a PluginType including
        /// Scoping, the Default Instance, and interception.  BuildInstancesOf()
        /// and ForRequestedType() are synonyms
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        [Obsolete("Change to For<T>()")]
        CreatePluginFamilyExpression<PLUGINTYPE> BuildInstancesOf<PLUGINTYPE>();

        /// <summary>
        /// Expression Builder used to define policies for a PluginType including
        /// Scoping, the Default Instance, and interception.  This method is specifically
        /// meant for registering open generic types
        /// </summary>
        /// <returns></returns>
        [Obsolete("Change to For(pluginType)")]
        GenericFamilyExpression ForRequestedType(Type pluginType);

        /// <summary>
        /// This method is a shortcut for specifying the default constructor and 
        /// setter arguments for a ConcreteType.  ForConcreteType is shorthand for:
        /// ForRequestedType[T]().TheDefault.Is.OfConcreteType[T].**************
        /// when the PluginType and ConcreteType are the same Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Registry.BuildWithExpression<T> ForConcreteType<T>();

        /// <summary>
        /// Expression Builder used to define policies for a PluginType including
        /// Scoping, the Default Instance, and interception.  BuildInstancesOf()
        /// and ForRequestedType() are synonyms
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        [Obsolete("Change to For<T>()")]
        CreatePluginFamilyExpression<PLUGINTYPE> ForRequestedType<PLUGINTYPE>();

        /// <summary>
        /// Convenience method.  Equivalent of ForRequestedType[PluginType]().AsSingletons()
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        CreatePluginFamilyExpression<PLUGINTYPE> ForSingletonOf<PLUGINTYPE>();

        /// <summary>
        /// Uses the configuration expressions of this Registry to create a PluginGraph
        /// object that could be used to initialize a Container.  This method is 
        /// mostly for internal usage, but might be helpful for diagnostics
        /// </summary>
        /// <returns></returns>
        PluginGraph Build();

        /// <summary>
        /// Adds an additional, non-Default Instance to the PluginType T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete("Prefer For<T>().Add() instead")]
        IsExpression<T> InstanceOf<T>();

        /// <summary>
        /// Adds an additional, non-Default Instance to the designated pluginType
        /// This method is mostly meant for open generic types
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        [Obsolete("Prefer For(type).Add() instead")]
        GenericIsExpression InstanceOf(Type pluginType);

        /// <summary>
        /// Expression Builder to define the defaults for a named Profile.  Each call
        /// to CreateProfile is additive.
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        ProfileExpression CreateProfile(string profileName);

        /// <summary>
        /// An alternative way to use CreateProfile that uses ProfileExpression
        /// as a Nested Closure.  This usage will result in cleaner code for 
        /// multiple declarations
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="action"></param>
        void CreateProfile(string profileName, Action<ProfileExpression> action);

        /// <summary>
        /// Registers a new TypeInterceptor object with the Container
        /// </summary>
        /// <param name="interceptor"></param>
        void RegisterInterceptor(TypeInterceptor interceptor);

        /// <summary>
        /// Allows you to define a TypeInterceptor inline with Lambdas or anonymous delegates
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        /// <example>
        /// IfTypeMatches( ... ).InterceptWith( o => new ObjectWrapper(o) );
        /// </example>
        MatchedTypeInterceptor IfTypeMatches(Predicate<Type> match);

        /// <summary>
        /// Designates a policy for scanning assemblies to auto
        /// register types
        /// </summary>
        /// <returns></returns>
        void Scan(Action<IAssemblyScanner> action);

        /// <summary>
        /// Directs StructureMap to always inject dependencies into any and all public Setter properties
        /// of the type PLUGINTYPE.
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        CreatePluginFamilyExpression<PLUGINTYPE> FillAllPropertiesOfType<PLUGINTYPE>();

        /// <summary>
        /// Creates automatic "policies" for which public setters are considered mandatory
        /// properties by StructureMap that will be "setter injected" as part of the 
        /// construction process.
        /// </summary>
        /// <param name="action"></param>
        void SetAllProperties(Action<SetterConvention> action);

        /// <summary>
        /// Use to programmatically select the constructor function of a concrete
        /// class.  Applies globally to all Containers in a single AppDomain.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        void SelectConstructor<T>(Expression<Func<T>> expression);

        /// <summary>
        /// All requests For the "TO" types will be filled by fetching the "FROM"
        /// type and casting it to "TO"
        /// GetInstance(typeof(TO)) basically becomes (TO)GetInstance(typeof(FROM))
        /// </summary>
        /// <typeparam name="FROM"></typeparam>
        /// <typeparam name="TO"></typeparam>
        void Forward<FROM, TO>() where FROM : class where TO : class;

        /// <summary>
        /// Syntactic Sugar for saying ForRequestedType().TheDefault.IsThis( @object )
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="object"></param>
        void Register<PLUGINTYPE>(PLUGINTYPE @object);

        /// <summary>
        /// Syntactic Sugar for saying ForRequestedType().TheDefault.IsThis( instance )
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="instance"></param>
        void Register<PLUGINTYPE>(Instance instance);

        /// <summary>
        /// Shorthand for ForRequestedType<PLUGINTYPE>()
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        CreatePluginFamilyExpression<PLUGINTYPE> For<PLUGINTYPE>();

        /// <summary>
        /// Shorthand for ForRequestedType(pluginType)
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        GenericFamilyExpression For(Type pluginType);

        /// <summary>
        /// Shortcut to make StructureMap return the default object of U casted to T
        /// whenever T is requested.  I.e.:
        /// For<T>().TheDefault.Is.ConstructedBy(c => c.GetInstance<U>() as T);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        LambdaInstance<T> Redirect<T, U>() where T : class where U : class;

        /// <summary>
        /// Advanced Usage Only!  Skips the Registry and goes right to the inner
        /// Semantic Model of StructureMap.  Use with care
        /// </summary>
        /// <param name="configure"></param>
        void Configure(Action<PluginGraph> configure);
    }

    /// <summary>
    /// A Registry class provides methods and grammars for configuring a Container or ObjectFactory.
    /// Using a Registry subclass is the recommended way of configuring a StructureMap Container.
    /// </summary>
    /// <example>
    /// public class MyRegistry : Registry
    /// {
    ///     public MyRegistry()
    ///     {
    ///         ForRequestedType(typeof(IService)).TheDefaultIsConcreteType(typeof(Service));
    ///     }
    /// }
    /// </example>
    public class Registry : IRegistry
    {
        private readonly List<Action<PluginGraph>> _actions = new List<Action<PluginGraph>>();
        private readonly List<Action> _basicActions = new List<Action>();

        /// <summary>
        /// Adds the concreteType as an Instance of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="concreteType"></param>
        public void AddType(Type pluginType, Type concreteType)
        {
            _actions.Add(g => g.AddType(pluginType, concreteType));
        }

        /// <summary>
        /// Adds the concreteType as an Instance of the pluginType with a name
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="concreteType"></param>
        /// <param name="name"></param>
        public void AddType(Type pluginType, Type concreteType, string name)
        {
            _actions.Add(g => g.AddType(pluginType, concreteType, name));
        }

        /// <summary>
        /// Add the pluggedType as an instance to any configured pluginType where pluggedType
        /// could be assigned to the pluginType
        /// </summary>
        /// <param name="pluggedType"></param>
        public void AddType(Type pluggedType)
        {
            _actions.Add(g => g.AddType(pluggedType));
        }

        /// <summary>
        /// Imports the configuration from another registry into this registry.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void IncludeRegistry<T>() where T : Registry, new()
        {
            _actions.Add(g => new T().ConfigurePluginGraph(g));
        }

        /// <summary>
        /// Imports the configuration from another registry into this registry.
        /// </summary>
        /// <param name="registry"></param>
        public void IncludeRegistry(Registry registry)
        {
            _actions.Add(registry.ConfigurePluginGraph);
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

            graph.Log.StartSource("Registry:  " + GetType().AssemblyQualifiedName);

            _basicActions.ForEach(action => action());
            _actions.ForEach(action => action(graph));

            graph.Registries.Add(this);
        }


        /// <summary>
        /// Expression Builder used to define policies for a PluginType including
        /// Scoping, the Default Instance, and interception.  BuildInstancesOf()
        /// and ForRequestedType() are synonyms
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        [Obsolete("Change to For<T>()")]
        public CreatePluginFamilyExpression<PLUGINTYPE> BuildInstancesOf<PLUGINTYPE>()
        {
            return new CreatePluginFamilyExpression<PLUGINTYPE>(this);
        }

        /// <summary>
        /// Expression Builder used to define policies for a PluginType including
        /// Scoping, the Default Instance, and interception.  This method is specifically
        /// meant for registering open generic types
        /// </summary>
        /// <returns></returns>
        [Obsolete("Change to For(pluginType)")]
        public GenericFamilyExpression ForRequestedType(Type pluginType)
        {
            return new GenericFamilyExpression(pluginType, this);
        }

        /// <summary>
        /// This method is a shortcut for specifying the default constructor and 
        /// setter arguments for a ConcreteType.  ForConcreteType is shorthand for:
        /// ForRequestedType[T]().TheDefault.Is.OfConcreteType[T].**************
        /// when the PluginType and ConcreteType are the same Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public BuildWithExpression<T> ForConcreteType<T>()
        {
            SmartInstance<T> instance = ForRequestedType<T>().TheDefault.Is.OfConcreteType<T>();
            return new BuildWithExpression<T>(instance);
        }

        /// <summary>
        /// Expression Builder used to define policies for a PluginType including
        /// Scoping, the Default Instance, and interception.  BuildInstancesOf()
        /// and ForRequestedType() are synonyms
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        [Obsolete("Change to For<T>()")]
        public CreatePluginFamilyExpression<PLUGINTYPE> ForRequestedType<PLUGINTYPE>()
        {
            return new CreatePluginFamilyExpression<PLUGINTYPE>(this);
        }

        /// <summary>
        /// Convenience method.  Equivalent of ForRequestedType[PluginType]().AsSingletons()
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> ForSingletonOf<PLUGINTYPE>()
        {
            return ForRequestedType<PLUGINTYPE>().Singleton();
        }

        /// <summary>
        /// Uses the configuration expressions of this Registry to create a PluginGraph
        /// object that could be used to initialize a Container.  This method is 
        /// mostly for internal usage, but might be helpful for diagnostics
        /// </summary>
        /// <returns></returns>
        public PluginGraph Build()
        {
            var graph = new PluginGraph();
            ConfigurePluginGraph(graph);
            graph.Seal();

            return graph;
        }

        /// <summary>
        /// Adds an additional, non-Default Instance to the PluginType T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete("Prefer For<T>().Add() instead")]
        public IsExpression<T> InstanceOf<T>()
        {
            return new InstanceExpression<T>(instance =>
            {
                Action<PluginGraph> alteration = g => g.FindFamily(typeof (T)).AddInstance(instance);
                _actions.Add(alteration);
            });
        }

        /// <summary>
        /// Adds an additional, non-Default Instance to the designated pluginType
        /// This method is mostly meant for open generic types
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        [Obsolete("Prefer For(type).Add() instead")]
        public GenericIsExpression InstanceOf(Type pluginType)
        {
            return
                new GenericIsExpression(
                    instance => { _actions.Add(graph => { graph.FindFamily(pluginType).AddInstance(instance); }); });
        }

        /// <summary>
        /// Expression Builder to define the defaults for a named Profile.  Each call
        /// to CreateProfile is additive.
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public ProfileExpression CreateProfile(string profileName)
        {
            var expression = new ProfileExpression(profileName, this);

            return expression;
        }

        /// <summary>
        /// An alternative way to use CreateProfile that uses ProfileExpression
        /// as a Nested Closure.  This usage will result in cleaner code for 
        /// multiple declarations
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="action"></param>
        public void CreateProfile(string profileName, Action<ProfileExpression> action)
        {
            var expression = new ProfileExpression(profileName, this);
            action(expression);
        }

        internal static bool IsPublicRegistry(Type type)
        {
            if (type.Assembly == typeof (Registry).Assembly)
            {
                return false;
            }

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
        /// Registers a new TypeInterceptor object with the Container
        /// </summary>
        /// <param name="interceptor"></param>
        public void RegisterInterceptor(TypeInterceptor interceptor)
        {
            addExpression(pluginGraph => pluginGraph.InterceptorLibrary.AddInterceptor(interceptor));
        }

        /// <summary>
        /// Allows you to define a TypeInterceptor inline with Lambdas or anonymous delegates
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        /// <example>
        /// IfTypeMatches( ... ).InterceptWith( o => new ObjectWrapper(o) );
        /// </example>
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

        /// <summary>
        /// Directs StructureMap to always inject dependencies into any and all public Setter properties
        /// of the type PLUGINTYPE.
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> FillAllPropertiesOfType<PLUGINTYPE>()
        {
            PluginCache.AddFilledType(typeof (PLUGINTYPE));
            return ForRequestedType<PLUGINTYPE>();
        }

        /// <summary>
        /// Creates automatic "policies" for which public setters are considered mandatory
        /// properties by StructureMap that will be "setter injected" as part of the 
        /// construction process.
        /// </summary>
        /// <param name="action"></param>
        public void SetAllProperties(Action<SetterConvention> action)
        {
            action(new SetterConvention());
        }

        /// <summary>
        /// Use to programmatically select the constructor function of a concrete
        /// class.  Applies globally to all Containers in a single AppDomain.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        public void SelectConstructor<T>(Expression<Func<T>> expression)
        {
            PluginCache.GetPlugin(typeof (T)).UseConstructor(expression);
        }

        /// <summary>
        /// All requests For the "TO" types will be filled by fetching the "FROM"
        /// type and casting it to "TO"
        /// GetInstance(typeof(TO)) basically becomes (TO)GetInstance(typeof(FROM))
        /// </summary>
        /// <typeparam name="FROM"></typeparam>
        /// <typeparam name="TO"></typeparam>
        public void Forward<FROM, TO>() where FROM : class where TO : class
        {
            For<TO>().AddInstances(x => x.ConstructedBy(c => c.GetInstance<FROM>() as TO));
        }


        /// <summary>
        /// Syntactic Sugar for saying ForRequestedType().TheDefault.IsThis( @object )
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="object"></param>
        public void Register<PLUGINTYPE>(PLUGINTYPE @object)
        {
            ForRequestedType<PLUGINTYPE>().TheDefault.IsThis(@object);
        }

        /// <summary>
        /// Syntactic Sugar for saying ForRequestedType().TheDefault.IsThis( instance )
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="instance"></param>
        public void Register<PLUGINTYPE>(Instance instance)
        {
            ForRequestedType<PLUGINTYPE>().TheDefault.IsThis(instance);
        }

        /// <summary>
        /// Shorthand for ForRequestedType<PLUGINTYPE>()
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> For<PLUGINTYPE>()
        {
            return new CreatePluginFamilyExpression<PLUGINTYPE>(this);
        }

        /// <summary>
        /// Shorthand for ForRequestedType(pluginType)
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public GenericFamilyExpression For(Type pluginType)
        {
            return ForRequestedType(pluginType);
        }


        /// <summary>
        /// Shortcut to make StructureMap return the default object of U casted to T
        /// whenever T is requested.  I.e.:
        /// For<T>().TheDefault.Is.ConstructedBy(c => c.GetInstance<U>() as T);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public LambdaInstance<T> Redirect<T, U>() where T : class where U : class
        {
            return For<T>().TheDefault.Is.ConstructedBy(c =>
            {
                var raw = c.GetInstance<U>();
                var t = raw as T;
                if (t == null)
                    throw new ApplicationException(raw.GetType().AssemblyQualifiedName + " could not be cast to " +
                                                   typeof (T).AssemblyQualifiedName);

                return t;
            });
        }

        /// <summary>
        /// Advanced Usage Only!  Skips the Registry and goes right to the inner
        /// Semantic Model of StructureMap.  Use with care
        /// </summary>
        /// <param name="configure"></param>
        public void Configure(Action<PluginGraph> configure)
        {
            _actions.Add(configure);
        }

        #region Nested type: BuildWithExpression

        /// <summary>
        /// Define the constructor and setter arguments for the default T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class BuildWithExpression<T>
        {
            private readonly SmartInstance<T> _instance;

            public BuildWithExpression(SmartInstance<T> instance)
            {
                _instance = instance;
            }

            public SmartInstance<T> Configure { get { return _instance; } }
        }

        #endregion
    }
}