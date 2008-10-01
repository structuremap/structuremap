using System;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
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

        public ConfiguredInstance OnCreation<TYPE>(Action<TYPE> handler)
        {
            StartupInterceptor<TYPE> interceptor = new StartupInterceptor<TYPE>(handler);
            Interceptor = interceptor;

            return this;
        }

        public ConfiguredInstance EnrichWith<TYPE>(EnrichmentHandler<TYPE> handler)
        {
            EnrichmentInterceptor<TYPE> interceptor = new EnrichmentInterceptor<TYPE>(handler);
            Interceptor = interceptor;

            return this;
        }


        public ChildArrayExpression ChildArray<PLUGINTYPE>(string propertyName)
        {
            validateTypeIsArray<PLUGINTYPE>();

            ChildArrayExpression expression =
                new ChildArrayExpression(this, propertyName);

            return expression;
        }

        public ChildArrayExpression ChildArray(string propertyName)
        {
            return new ChildArrayExpression(this, propertyName);
        }

        public ChildArrayExpression ChildArray<PLUGINTYPE>()
        {
            validateTypeIsArray<PLUGINTYPE>();

            string propertyName = findPropertyName<PLUGINTYPE>();
            return ChildArray<PLUGINTYPE>(propertyName);
        }

        /// <summary>
        /// Start the definition of a child instance for type CONSTRUCTORARGUMENTTYPE
        /// </summary>
        /// <typeparam name="CONSTRUCTORARGUMENTTYPE"></typeparam>
        /// <returns></returns>
        public ChildInstanceExpression Child<CONSTRUCTORARGUMENTTYPE>()
        {
            string propertyName = findPropertyName<CONSTRUCTORARGUMENTTYPE>();

            ChildInstanceExpression child = Child(propertyName);
            child.ChildType = typeof (CONSTRUCTORARGUMENTTYPE);

            return child;
        }

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
            ChildInstanceExpression child = new ChildInstanceExpression(this, propertyName);
            child.ChildType = typeof (CONSTRUCTORARGUMENTTYPE);

            return child;
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

            public ConfiguredInstance Contains(params Instance[] instances)
            {
                _instance.setChildArray(_propertyName, instances);

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

            internal Type ChildType
            {
                set { _childType = value; }
            }

            /// <summary>
            /// Use a previously configured and named instance for the child
            /// </summary>
            /// <param name="instanceKey"></param>
            /// <returns></returns>
            public ConfiguredInstance IsNamedInstance(string instanceKey)
            {
                ReferencedInstance instance = new ReferencedInstance(instanceKey);
                _instance.setChild(_propertyName, instance);

                return _instance;
            }

            /// <summary>
            /// Start the definition of a child instance by defining the concrete type
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public ConfiguredInstance IsConcreteType<T>()
            {
                Type pluggedType = typeof (T);
                ExpressionValidator.ValidatePluggabilityOf(pluggedType).IntoPluginType(_childType);

                ConfiguredInstance childInstance = new ConfiguredInstance(pluggedType);
                _instance.setChild(_propertyName, childInstance);

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
                _instance.setChild(_propertyName, child);
                return _instance;
            }

            public ConfiguredInstance Is(object value)
            {
                LiteralInstance instance = new LiteralInstance(value);
                return Is(instance);
            }

            public ConfiguredInstance IsAutoFilled()
            {
                DefaultInstance instance = new DefaultInstance();
                return Is(instance);
            }
        }

        #endregion

        #region Nested type: PropertyExpression

        #endregion
    }
}