using System;
using StructureMap.Configuration.DSL;
using StructureMap.Interceptors;

namespace StructureMap.Pipeline
{
    public partial class ConfiguredInstance
    {
        public ConfiguredInstance WithName(string instanceKey)
        {
            Name = instanceKey;
            return this;
        }

        /// <summary>
        /// Register an Action to perform on the object created by this Instance
        /// before it is returned to the caller
        /// </summary>
        /// <typeparam name="TYPE"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ConfiguredInstance OnCreation<TYPE>(Action<TYPE> handler)
        {
            var interceptor = new StartupInterceptor<TYPE>((c, o) => handler(o));
            Interceptor = interceptor;

            return this;
        }

        /// <summary>
        /// Register an Action to perform on the object created by this Instance
        /// before it is returned to the caller
        /// </summary>
        /// <typeparam name="TYPE"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ConfiguredInstance OnCreation<TYPE>(Action<IContext, TYPE> handler)
        {
            var interceptor = new StartupInterceptor<TYPE>(handler);
            Interceptor = interceptor;

            return this;
        }

        /// <summary>
        /// Register a Func to potentially enrich or substitute for the object
        /// created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ConfiguredInstance EnrichWith<TYPE>(EnrichmentHandler<TYPE> handler)
        {
            var interceptor = new EnrichmentInterceptor<TYPE>((c, o) => handler(o));
            Interceptor = interceptor;

            return this;
        }

        /// <summary>
        /// Register a Func to potentially enrich or substitute for the object
        /// created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ConfiguredInstance EnrichWith<TYPE>(ContextEnrichmentHandler<TYPE> handler)
        {
            var interceptor = new EnrichmentInterceptor<TYPE>(handler);
            Interceptor = interceptor;

            return this;
        }

        /// <summary>
        /// Inline definition of a dependency array like IService[] or IHandler[]
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public ChildArrayExpression ChildArray<PLUGINTYPE>(string propertyName)
        {
            var expression =
                new ChildArrayExpression(this, propertyName);

            return expression;
        }

        /// <summary>
        /// Inline definition of a dependency array like IService[] or IHandler[]
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public ChildArrayExpression ChildArray(string propertyName)
        {
            return new ChildArrayExpression(this, propertyName);
        }

        public ChildArrayExpression ChildArray(Type pluginType)
        {
            string propertyName = findPropertyName(pluginType);
            return ChildArray(propertyName);
        }

        /// <summary>
        /// Inline definition of a dependency array like IService[] or IHandler[]
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        public ChildArrayExpression ChildArray<PLUGINTYPE>()
        {
            return ChildArray(typeof (PLUGINTYPE));
        }

        /// <summary>
        /// Start the definition of a child instance for type CONSTRUCTORARGUMENTTYPE
        /// </summary>
        /// <typeparam name="CONSTRUCTORARGUMENTTYPE"></typeparam>
        /// <returns></returns>
        public ChildInstanceExpression Child<CONSTRUCTORARGUMENTTYPE>()
        {
            Type dependencyType = typeof (CONSTRUCTORARGUMENTTYPE);

            return Child(dependencyType);
        }

        /// <summary>
        /// Start the definition of a child instance for type CONSTRUCTORARGUMENTTYPE
        /// </summary>
        /// <typeparam name="CONSTRUCTORARGUMENTTYPE"></typeparam>
        /// <returns></returns>
        public ChildInstanceExpression Child(Type dependencyType)
        {
            string propertyName = findPropertyName(dependencyType);

            ChildInstanceExpression child = Child(propertyName);
            child.ChildType = dependencyType;

            return child;
        }


        /// <summary>
        /// Inline definition of a constructor or a setter property dependency
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public ChildInstanceExpression Child(string propertyName)
        {
            return new ChildInstanceExpression(this, propertyName);
        }


        /// <summary>
        /// Starts the definition of a child instance specifying the argument name
        /// in the case of a constructor function that consumes more than one argument
        /// of type T
        /// </summary>
        /// <typeparam name="CONSTRUCTORARGUMENTTYPE"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public ChildInstanceExpression Child<CONSTRUCTORARGUMENTTYPE>(string propertyName)
        {
            var child = new ChildInstanceExpression(this, propertyName);
            child.ChildType = typeof (CONSTRUCTORARGUMENTTYPE);

            return child;
        }

        /// <summary>
        /// Inline definition of a constructor dependency
        /// </summary>
        /// <typeparam name="CONSTRUCTORARGUMENTTYPE"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public ChildInstanceExpression CtorDependency<CONSTRUCTORARGUMENTTYPE>(string propertyName)
        {
            return Child<CONSTRUCTORARGUMENTTYPE>(propertyName);
        }

        /// <summary>
        /// Inline definition of a setter dependency
        /// </summary>
        /// <typeparam name="CONSTRUCTORARGUMENTTYPE"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public ChildInstanceExpression SetterDependency<CONSTRUCTORARGUMENTTYPE>(string propertyName)
        {
            return Child<CONSTRUCTORARGUMENTTYPE>(propertyName);
        }

        /// <summary>
        /// Start the definition of a primitive argument to a constructor argument
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public PropertyExpression<ConfiguredInstance> WithProperty(string propertyName)
        {
            return new PropertyExpression<ConfiguredInstance>(this, propertyName);
        }

        /// <summary>
        /// Configure a primitive constructor argument
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public PropertyExpression<ConfiguredInstance> WithCtorArg(string propertyName)
        {
            return new PropertyExpression<ConfiguredInstance>(this, propertyName);
        }

        #region Nested type: ChildArrayExpression

        public class ChildArrayExpression
        {
            private readonly ConfiguredInstance _instance;
            private readonly string _propertyName;

            public ChildArrayExpression(ConfiguredInstance instance, string propertyName)
            {
                _instance = instance;
                _propertyName = propertyName;
            }

            /// <summary>
            /// Configures an array of Instance's for the array dependency
            /// </summary>
            /// <param name="instances"></param>
            /// <returns></returns>
            public ConfiguredInstance Contains(params Instance[] instances)
            {
                _instance.SetCollection(_propertyName, instances);

                return _instance;
            }
        }

        #endregion

        #region Nested type: ChildInstanceExpression

        /// <summary>
        /// Part of the Fluent Interface, represents a nonprimitive argument to a 
        /// constructure function
        /// </summary>
        public class ChildInstanceExpression
        {
            private readonly ConfiguredInstance _instance;
            private readonly string _propertyName;
            private Type _childType;


            public ChildInstanceExpression(ConfiguredInstance instance, string propertyName)
            {
                _instance = instance;
                _propertyName = propertyName;
            }

            public ChildInstanceExpression(ConfiguredInstance instance, string propertyName,
                                           Type childType)
                : this(instance, propertyName)
            {
                _childType = childType;
            }

            internal Type ChildType { set { _childType = value; } }

            /// <summary>
            /// Use a previously configured and named instance for the child
            /// </summary>
            /// <param name="instanceKey"></param>
            /// <returns></returns>
            public ConfiguredInstance IsNamedInstance(string instanceKey)
            {
                var instance = new ReferencedInstance(instanceKey);
                _instance.SetChild(_propertyName, instance);

                return _instance;
            }

            /// <summary>
            /// Start the definition of a child instance by defining the concrete type
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public ConfiguredInstance IsConcreteType<T>()
            {
                return IsConcreteType(typeof (T));
            }

            /// <summary>
            /// Start the definition of a child instance by defining the concrete type
            /// </summary>
            /// <param name="pluggedType"></param>
            /// <returns></returns>
            public ConfiguredInstance IsConcreteType(Type pluggedType)
            {
                ExpressionValidator.ValidatePluggabilityOf(pluggedType).IntoPluginType(_childType);

                var childInstance = new ConfiguredInstance(pluggedType);
                _instance.SetChild(_propertyName, childInstance);

                return _instance;
            }

            /// <summary>
            /// Registers a configured instance to use as the argument to the parent's
            /// constructor
            /// </summary>
            /// <param name="child"></param>
            /// <returns></returns>
            public ConfiguredInstance Is(Instance child)
            {
                _instance.SetChild(_propertyName, child);
                return _instance;
            }

            public ConfiguredInstance Is(object value)
            {
                var instance = new ObjectInstance(value);
                return Is(instance);
            }

            /// <summary>
            /// Directs StructureMap to fill this dependency with the Default Instance of the 
            /// constructor or property type
            /// </summary>
            /// <returns></returns>
            public ConfiguredInstance IsAutoFilled()
            {
                var instance = new DefaultInstance();
                return Is(instance);
            }
        }

        #endregion
    }
}