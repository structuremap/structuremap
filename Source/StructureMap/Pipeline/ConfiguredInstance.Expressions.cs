using System;
using System.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public partial class ConfiguredInstance
    {
        /// <summary>
        /// Use a named Plugin type denoted by a [Pluggable("Key")] attribute
        /// </summary>
        /// <param name="concreteKey"></param>
        /// <returns></returns>
        public ConfiguredInstance UsingConcreteTypeNamed(string concreteKey)
        {
            _concreteKey = concreteKey;
            return this;
        }

        /// <summary>
        /// Use type T for the concrete type of an instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ConfiguredInstance UsingConcreteType<T>()
        {
            _pluggedType = typeof (T);
            return this;
        }

        public ChildArrayExpression ChildArray<PLUGINTYPE>(string propertyName)
        {
            validateTypeIsArray<PLUGINTYPE>();

            ChildArrayExpression expression =
                new ChildArrayExpression(this, propertyName, typeof (PLUGINTYPE));

            return expression;
        }

        public ChildArrayExpression ChildArray(string propertyName, Type childType)
        {
            return new ChildArrayExpression(this, propertyName, childType);
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
        public PropertyExpression WithProperty(string propertyName)
        {
            return new PropertyExpression(this, propertyName);
        }

        public ConfiguredInstance WithConcreteKey(string concreteKey)
        {
            replaceNameIfNotAlreadySet(concreteKey);
            _concreteKey = concreteKey;
            return this;
        }


        private string findPropertyName<T>()
        {
            Plugin plugin = new Plugin(_pluggedType);
            string propertyName = plugin.FindFirstConstructorArgumentOfType<T>();

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new StructureMapException(305, typeof (T));
            }

            return propertyName;
        }

        private static void validateTypeIsArray<PLUGINTYPE>()
        {
            if (!typeof (PLUGINTYPE).IsArray)
            {
                throw new StructureMapException(307);
            }
        }

        #region Nested type: ChildArrayExpression

        public class ChildArrayExpression
        {
            private readonly Type _childType;
            private readonly ConfiguredInstance _instance;
            private readonly string _propertyName;

            public ChildArrayExpression(ConfiguredInstance instance, string propertyName, Type childType)
            {
                _instance = instance;
                _propertyName = propertyName;

                _childType = _childType;
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

                ConfiguredInstance childInstance = new ConfiguredInstance();
                childInstance._pluggedType = pluggedType;
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
        }

        #endregion

        #region Nested type: PropertyExpression

        /// <summary>
        /// Defines the value of a primitive argument to a constructur argument
        /// </summary>
        public class PropertyExpression
        {
            private readonly ConfiguredInstance _instance;
            private readonly string _propertyName;

            public PropertyExpression(ConfiguredInstance instance, string propertyName)
            {
                _instance = instance;
                _propertyName = propertyName;
            }

            /// <summary>
            /// Sets the value of the constructor argument
            /// </summary>
            /// <param name="propertyValue"></param>
            /// <returns></returns>
            public ConfiguredInstance EqualTo(object propertyValue)
            {
                _instance.SetProperty(_propertyName, propertyValue.ToString());
                return _instance;
            }

            /// <summary>
            /// Sets the value of the constructor argument to the key/value in the 
            /// AppSettings
            /// </summary>
            /// <param name="appSettingKey"></param>
            /// <returns></returns>
            public ConfiguredInstance EqualToAppSetting(string appSettingKey)
            {
                string propertyValue = ConfigurationManager.AppSettings[appSettingKey];
                _instance.SetProperty(_propertyName, propertyValue);
                return _instance;
            }
        }

        #endregion
    }
}