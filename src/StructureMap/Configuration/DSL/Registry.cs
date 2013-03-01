using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL
{
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
    public class Registry : IRegistry, IPluginGraphConfiguration
    {
        private readonly List<Action<PluginGraph>> _actions = new List<Action<PluginGraph>>();
        private readonly List<Action> _basicActions = new List<Action>();
        private readonly List<Action<PluginGraphBuilder>> _builders = new List<Action<PluginGraphBuilder>>();

        internal Action<PluginGraph> alter
        {
            set
            {
                _actions.Add(value);
            }
        }

        private Action<PluginGraphBuilder> register
        {
            set
            {
                _builders.Add(value);
            }
        }

        /// <summary>
        /// Adds the concreteType as an Instance of the pluginType.  Mostly useful
        /// for conventions
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="concreteType"></param>
        public void AddType(Type pluginType, Type concreteType)
        {
            alter = g => g.AddType(pluginType, concreteType);
        }

        /// <summary>
        /// Adds the concreteType as an Instance of the pluginType with a name.  Mostly
        /// useful for conventions
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="concreteType"></param>
        /// <param name="name"></param>
        public virtual void AddType(Type pluginType, Type concreteType, string name)
        {
            alter = g => g.AddType(pluginType, concreteType, name);
        }

        /// <summary>
        /// Imports the configuration from another registry into this registry.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void IncludeRegistry<T>() where T : Registry, new()
        {
            IncludeRegistry(new T());
        }

        /// <summary>
        /// Imports the configuration from another registry into this registry.
        /// </summary>
        /// <param name="registry"></param>
        public void IncludeRegistry(Registry registry)
        {
            alter = graph => {
                registry.As<IPluginGraphConfiguration>().Configure(graph);
            };
        }

        /// <summary>
        /// This method is a shortcut for specifying the default constructor and 
        /// setter arguments for a ConcreteType.  ForConcreteType is shorthand for:
        /// For[T]().Use[T].**************
        /// when the PluginType and ConcreteType are the same Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public BuildWithExpression<T> ForConcreteType<T>()
        {
            SmartInstance<T> instance = For<T>().Use<T>();
            return new BuildWithExpression<T>(instance);
        }

        /// <summary>
        /// Convenience method.  Equivalent of ForRequestedType[PluginType]().Singletons()
        /// </summary>
        /// <typeparam name="TPluginType"></typeparam>
        /// <returns></returns>
        public CreatePluginFamilyExpression<TPluginType> ForSingletonOf<TPluginType>()
        {
            return For<TPluginType>().Singleton();
        }

        /// <summary>
        /// Shorthand way of saying For(pluginType).Singleton()
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public GenericFamilyExpression ForSingletonOf(Type pluginType)
        {
            return For(pluginType).Singleton();
        }

        /// <summary>
        /// An alternative way to use CreateProfile that uses ProfileExpression
        /// as a Nested Closure.  This usage will result in cleaner code for 
        /// multiple declarations
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="action"></param>
        public void Profile(string profileName, Action<Registry> action)
        {
            var registry = new Registry();
            action(registry);

            alter = x => registry.As<IPluginGraphConfiguration>().Configure(x.Profile(profileName));
        }

        /// <summary>
        /// Registers a new TypeInterceptor object with the Container
        /// </summary>
        /// <param name="interceptor"></param>
        public void RegisterInterceptor(TypeInterceptor interceptor)
        {
            alter = x => x.InterceptorLibrary.AddInterceptor(interceptor);
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
            alter = graph => graph.InterceptorLibrary.AddInterceptor(interceptor);

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

            register = x => x.AddScanner(scanner);
        }

        /// <summary>
        /// Directs StructureMap to always inject dependencies into any and all public Setter properties
        /// of the type TPluginType.
        /// </summary>
        /// <typeparam name="TPluginType"></typeparam>
        /// <returns></returns>
        public CreatePluginFamilyExpression<TPluginType> FillAllPropertiesOfType<TPluginType>()
        {
            PluginCache.AddFilledType(typeof (TPluginType));
            return For<TPluginType>();
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
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        public void Forward<TFrom, TTo>() where TFrom : class where TTo : class
        {
            For<TTo>().AddInstances(x => x.ConstructedBy(c => c.GetInstance<TFrom>() as TTo));
        }

        /// <summary>
        /// Expression Builder used to define policies for a PluginType including
        /// Scoping, the Default Instance, and interception.  BuildInstancesOf()
        /// and ForRequestedType() are synonyms
        /// </summary>
        /// <typeparam name="TPluginType"></typeparam>
        /// <param name="scope">Optionally specify the instance scoping for this PluginType</param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<TPluginType> For<TPluginType>(string scope = null)
        {
            return new CreatePluginFamilyExpression<TPluginType>(this, scope);
        }

        /// <summary>
        /// Expression Builder used to define policies for a PluginType including
        /// Scoping, the Default Instance, and interception.  This method is specifically
        /// meant for registering open generic types
        /// </summary>
        /// <param name="scope">Optionally specify the instance scoping for this PluginType</param>
        /// <returns></returns>
        public GenericFamilyExpression For(Type pluginType, string scope = null)
        {
            return new GenericFamilyExpression(pluginType, scope, this);
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
            return For<T>().Use(c =>
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
            alter = configure;
        }

        protected void registerAction(Action action)
        {
            _basicActions.Add(action);
        }

        void IPluginGraphConfiguration.Configure(PluginGraph graph)
        {
            if (graph.Registries.Contains(this)) return;

            graph.Log.StartSource("Registry:  " + GetType().AssemblyQualifiedName);

            _basicActions.ForEach(action => action());
            _actions.ForEach(action => action(graph));

            graph.Registries.Add(this);
        }

        void IPluginGraphConfiguration.Register(PluginGraphBuilder builder)
        {
            _builders.Each(x => x(builder));
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

        public bool Equals(Registry other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if(other.GetType() == typeof(Registry) && GetType() == typeof(Registry)) return false;
            if (Equals(other.GetType(), GetType()))
            {
                return !GetType().IsNotPublic;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!typeof (Registry).IsAssignableFrom(obj.GetType())) return false;
            return Equals((Registry) obj);
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

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

        /// <summary>
        /// Gives a <see cref="IPluginGraphConfiguration"/> the possibility to interact with the current <see cref="PluginGraphBuilder"/>
        /// via <see cref="IPluginGraphConfiguration.Register"/>.
        /// </summary>
        public void RegisterPluginGraphConfiguration<T>() where T : IPluginGraphConfiguration, new()
        {
            RegisterPluginGraphConfiguration(new T());
        }

        /// <summary>
        /// See <see cref="RegisterPluginGraphConfiguration{T}"/>
        /// </summary>
        public void RegisterPluginGraphConfiguration(IPluginGraphConfiguration pluginGraphConfig)
        {
            register = pluginGraphConfig.Register;
        }

        /// <summary>
        /// Gives a <see cref="IPluginGraphConfiguration"/> the possibility to interact with the resulting <see cref="PluginGraph"/>,
        /// i.e. as opposed to <see cref="RegisterPluginGraphConfiguration"/>, the PluginGraph is built, and the provided
        /// PluginGraph config obtains access to saig graph.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ConfigurePluginGraph<T>() where T : IPluginGraphConfiguration, new()
        {
            ConfigurePluginGraph(new T());
        }

        /// <summary>
        /// <see cref="ConfigurePluginGraph{T}"/>
        /// </summary>
        public void ConfigurePluginGraph(IPluginGraphConfiguration pluginGraphConfig)
        {
            alter = pluginGraphConfig.Configure;
        }
    }
}