using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap.Building.Interception;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap
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
    public class Registry : IRegistry
    {
        private readonly IList<Action<PluginGraph>> _actions = new List<Action<PluginGraph>>();

        internal readonly IList<AssemblyScanner> Scanners = new List<AssemblyScanner>(); 

        internal Action<PluginGraph> alter
        {
            set { _actions.Add(value); }
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
            foreach (var scanner in registry.Scanners)
            {
                Scanners.Add(scanner);
            }

            foreach (var action in registry._actions)
            {
                _actions.Add(action);
            }
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

            alter = x => registry.Configure(x.Profile(profileName));
        }


        /// <summary>
        /// Designates a policy for scanning assemblies to auto
        /// register types
        /// </summary>
        /// <returns></returns>
        public void Scan(Action<IAssemblyScanner> action)
        {
            var scanner = new AssemblyScanner();

            if (GetType().GetTypeInfo().Assembly == typeof(Registry).GetTypeInfo().Assembly)
            {
                scanner.Description = "Scanner #" + (Scanners.Count + 1);
            }
            else
            {
                scanner.Description = "{0} Scanner #{1}".ToFormat(GetType().Name, (Scanners.Count + 1));
            }

            action(scanner);

            Scanners.Add(scanner);
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
        public LambdaInstance<T, T> Redirect<T, U>() where T : class where U : class
        {
            return
                For<T>()
                    .Use(
                        "Redirect requests for {0} to the configured default of {1} with a cast".ToFormat(
                            typeof (T).GetFullName(), typeof (U).GetFullName()), c => {
                                var raw = c.GetInstance<U>();
                                var t = raw as T;
                                if (t == null)
                                    throw new InvalidCastException(raw.GetType().AssemblyQualifiedName +
                                                                   " could not be cast to " +
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

        internal void Configure(PluginGraph graph)
        {
            _actions.Each(action => action(graph));
        }

        public TransientTracking TransientTracking
        {
            set { alter = graph => graph.TransientTracking = value; }
        }

        internal static bool IsPublicRegistry(Type type)
        {
            var ti = type.GetTypeInfo();
            if (Equals(ti.Assembly, typeof (Registry).GetTypeInfo().Assembly))
            {
                return false;
            }

            if (!typeof (Registry).GetTypeInfo().IsAssignableFrom(ti))
            {
                return false;
            }

            if (ti.IsInterface || ti.IsAbstract || ti.IsGenericType)
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
            if (other.GetType() == GetType() || other.GetType().AssemblyQualifiedName == GetType().AssemblyQualifiedName)
            {
                return !GetType().GetTypeInfo().IsNotPublic;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!typeof(Registry).GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo())) return false;
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
            private readonly SmartInstance<T, T> _instance;

            public BuildWithExpression(SmartInstance<T, T> instance)
            {
                _instance = instance;
            }

            public SmartInstance<T, T> Configure
            {
                get { return _instance; }
            }
        }

        /// <summary>
        /// Configure Container-wide policies and conventions
        /// </summary>
        public PoliciesExpression Policies
        {
            get { return new PoliciesExpression(this); }
        }

        public class PoliciesExpression
        {
            private readonly Registry _parent;

            private Action<PluginGraph> alter
            {
                set
                {
                    _parent.PoliciesChanged = true;
                    _parent.alter = value;
                }
            }

            public PoliciesExpression(Registry parent)
            {
                _parent = parent;
            }

            /// <summary>
            /// Adds a new instance policy to this container
            /// that can apply to every object instance created
            /// by this container
            /// </summary>
            /// <param name="policy"></param>
            public void Add(IInstancePolicy policy)
            {
                alter = graph => graph.Policies.Add(policy);
            }

            /// <summary>
            /// Adds a new instance policy to this container
            /// that can apply to every object instance created
            /// by this container
            /// </summary>
            public void Add<T>() where T : IInstancePolicy, new()
            {
                Add(new T());
            }

            /// <summary>
            /// Register an interception policy
            /// </summary>
            /// <param name="policy"></param>
            public void Interceptors(IInterceptorPolicy policy)
            {
                alter = graph => graph.Policies.Add(policy);
            }

            /// <summary>
            /// Register a strategy for automatically resolving "missing" families
            /// when an unknown PluginType is first encountered
            /// </summary>
            /// <typeparam name="T"></typeparam>
            public void OnMissingFamily<T>() where T : IFamilyPolicy, new()
            {
                alter = graph => graph.AddFamilyPolicy(new T());
            }

            /// <summary>
            /// Register a strategy for automatically resolving "missing" families
            /// when an unknown PluginType is first encountered
            /// </summary>
            /// <param name="policy"></param>
            public void OnMissingFamily(IFamilyPolicy policy)
            {
                alter = graph => graph.AddFamilyPolicy(policy);
            }

            /// <summary>
            /// Register a custom constructor selection policy
            /// </summary>
            /// <typeparam name="T"></typeparam>
            public void ConstructorSelector<T>() where T : IConstructorSelector, new()
            {
                ConstructorSelector(new T());
            }

            /// <summary>
            /// Register a custom constructor selection policy
            /// </summary>
            /// <param name="constructorSelector"></param>
            public void ConstructorSelector(IConstructorSelector constructorSelector)
            {
                alter = x => x.Policies.ConstructorSelector.Add(constructorSelector);
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

                alter = graph => convention.As<SetterConventionRule>().Configure(graph.Policies.SetterRules);
            }

            /// <summary>
            /// Directs StructureMap to always inject dependencies into any and all public Setter properties
            /// of the type TPluginType.
            /// </summary>
            /// <typeparam name="TPluginType"></typeparam>
            /// <returns></returns>
            public CreatePluginFamilyExpression<TPluginType> FillAllPropertiesOfType<TPluginType>()
            {
                Func<PropertyInfo, bool> predicate = prop => prop.PropertyType == typeof (TPluginType);

                alter = graph => graph.Policies.SetterRules.Add(predicate);

                return _parent.For<TPluginType>();
            }
        }

        internal bool PoliciesChanged { get; set; }


        private static int mutation = 0;

        public static bool RegistryExists(IEnumerable<Registry> all, Registry registry)
        {
            if (all.Contains(registry)) return true;

            var type = registry.GetType();

            if (type == typeof (Registry)) return false;
            if (type == typeof (ConfigurationExpression)) return false;

            var constructors = type.GetConstructors();
            if (constructors.Count() == 1 && !constructors.Single().GetParameters().Any())
            {
                if (all.Any(x => x.GetType() == type)) return true;
                if (all.Any(x => x.GetType().AssemblyQualifiedName == type.AssemblyQualifiedName)) return true;
            }

            return false;
        }
    }
}