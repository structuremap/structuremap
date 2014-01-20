using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

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
            set { _actions.Add(value); }
        }

        private Action<PluginGraphBuilder> register
        {
            set { _builders.Add(value); }
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
            alter = graph => { registry.As<IPluginGraphConfiguration>().Configure(graph); };
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
            var instance = For<T>().Use<T>();
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
        public void Profile(string profileName, Action<IProfileRegistry> action)
        {
            var registry = new Registry();
            action(registry);

            alter = x => registry.As<IPluginGraphConfiguration>().Configure(x.Profile(profileName));
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
        /// <param name="lifecycle">Optionally specify the instance scoping for this PluginType</param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<TPluginType> For<TPluginType>(ILifecycle lifecycle = null)
        {
            return new CreatePluginFamilyExpression<TPluginType>(this, lifecycle);
        }

        /// <summary>
        /// Expression Builder used to define policies for a PluginType including
        /// Scoping, the Default Instance, and interception.  This method is specifically
        /// meant for registering open generic types
        /// </summary>
        /// <param name="lifecycle">Optionally specify the instance scoping for this PluginType</param>
        /// <returns></returns>
        public GenericFamilyExpression For(Type pluginType, ILifecycle lifecycle = null)
        {
            if (lifecycle == null)
            {
                lifecycle = Lifecycles.Transient;
            }
            return new GenericFamilyExpression(pluginType, lifecycle, this);
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
            return For<T>().Use("Redirect requests for {0} to the configured default of {1} with a cast".ToFormat(typeof(T).GetFullName(), typeof(U).GetFullName()),c => {
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
            if (other.GetType() == typeof (Registry) && GetType() == typeof (Registry)) return false;
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

            public SmartInstance<T> Configure
            {
                get { return _instance; }
            }
        }






        public PoliciesExpression Polices
        {
            get
            {
                return new PoliciesExpression(this);
            }
        }

        // TODO -- add Xml comments
        public class PoliciesExpression
        {
            private readonly Registry _parent;

            public PoliciesExpression(Registry parent)
            {
                _parent = parent;
            }

            public void OnMissingFamily<T>() where T : IFamilyPolicy, new()
            {
                _parent.alter = graph => graph.AddFamilyPolicy(new T());
            }

            /// <summary>
            /// <see cref="Configure{T}"/>
            /// </summary>
            public void Configure(IPluginGraphConfiguration pluginGraphConfig)
            {
                _parent.alter = pluginGraphConfig.Configure;
                _parent.register = pluginGraphConfig.Register;
            }

            public void ConstructorSelector<T>() where T : IConstructorSelector, new()
            {
                ConstructorSelector(new T());
            }

            public void ConstructorSelector(IConstructorSelector constructorSelector)
            {
                _parent.alter = x => x.Policies.ConstructorSelector.Add(constructorSelector);
            }

            /// <summary>
            /// Gives a <see cref="IPluginGraphConfiguration"/> the possibility to interact with the resulting <see cref="PluginGraph"/>,
            /// i.e. as opposed to <see cref="RegisterPluginGraphConfiguration"/>, the PluginGraph is built, and the provided
            /// PluginGraph config obtains access to saig graph.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            public void Configure<T>() where T : IPluginGraphConfiguration, new()
            {
                Configure(new T());
            }


            /// <summary>
            /// Creates automatic "policies" for which public setters are considered mandatory
            /// properties by StructureMap that will be "setter injected" as part of the 
            /// construction process.
            /// </summary>
            /// <param name="action"></param>
            public void SetAllProperties(Action<SetterConvention> action)
            {
                var convention = new SetterConvention();
                action(convention);

                _parent.alter = graph => convention.As<SetterConventionRule>().Configure(graph.Policies.SetterRules);
            }

            /// <summary>
            /// Directs StructureMap to always inject dependencies into any and all public Setter properties
            /// of the type TPluginType.
            /// </summary>
            /// <typeparam name="TPluginType"></typeparam>
            /// <returns></returns>
            public CreatePluginFamilyExpression<TPluginType> FillAllPropertiesOfType<TPluginType>()
            {
                Func<PropertyInfo, bool> predicate = prop => prop.PropertyType == typeof(TPluginType);

                _parent.alter = graph => graph.Policies.SetterRules.Add(predicate);

                return _parent.For<TPluginType>();
            }
        }
    }
}