using System;
using StructureMap.Configuration.Mementos;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL.Expressions
{
    /// <summary>
    /// Used to define an Instance in code
    /// </summary>
    public class InstanceExpression : MementoBuilder<InstanceExpression>
    {
        private MemoryInstanceMemento _memento;
        private Type _pluggedType;

        public InstanceExpression(Type pluginType) : base(pluginType)
        {
        }


        internal Type PluggedType
        {
            get { return _pluggedType; }
        }


        protected override InstanceMemento memento
        {
            get { return _memento; }
        }

        protected override InstanceExpression thisInstance
        {
            get { return this; }
        }

        protected override void buildMemento()
        {
            _memento = new MemoryInstanceMemento();
        }

        protected override void configureMemento(PluginFamily family)
        {
            Plugin plugin = _pluggedType == null
                                ? family.Plugins[_memento.ConcreteKey]
                                : family.Plugins.FindOrCreate(_pluggedType);

            _memento.ConcreteKey = plugin.ConcreteKey;
        }

        protected override void validate()
        {
            if (_pluggedType == null && string.IsNullOrEmpty(_memento.ConcreteKey))
            {
                throw new StructureMapException(301, _memento.InstanceKey,
                                                TypePath.GetAssemblyQualifiedName(_pluginType));
            }
        }


        /// <summary>
        /// Start the definition of a primitive argument to a constructor argument
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public PropertyExpression WithProperty(string propertyName)
        {
            return new PropertyExpression(this, _memento, propertyName);
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
            ChildInstanceExpression child = new ChildInstanceExpression(this, _memento, propertyName);
            addChildExpression(child);
            child.ChildType = typeof (CONSTRUCTORARGUMENTTYPE);

            return child;
        }

        /// <summary>
        /// Start the definition of a child instance for type CONSTRUCTORARGUMENTTYPE
        /// </summary>
        /// <typeparam name="CONSTRUCTORARGUMENTTYPE"></typeparam>
        /// <returns></returns>
        public ChildInstanceExpression Child<CONSTRUCTORARGUMENTTYPE>()
        {
            string propertyName = findPropertyName<CONSTRUCTORARGUMENTTYPE>();

            ChildInstanceExpression child = new ChildInstanceExpression(this, _memento, propertyName);
            addChildExpression(child);
            child.ChildType = typeof (CONSTRUCTORARGUMENTTYPE);
            return child;
        }

        private string findPropertyName<T>()
        {
            Plugin plugin = Plugin.CreateImplicitPlugin(_pluggedType);
            string propertyName = plugin.FindFirstConstructorArgumentOfType<T>();

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new StructureMapException(305, TypePath.GetAssemblyQualifiedName(typeof (T)));
            }

            return propertyName;
        }

        public override void ValidatePluggability(Type pluginType)
        {
            if (_pluggedType == null)
            {
                return;
            }

            ExpressionValidator.ValidatePluggabilityOf(_pluggedType).IntoPluginType(pluginType);
        }

        internal InstanceTypeExpression TypeExpression()
        {
            return new InstanceTypeExpression(this);
        }

        public ChildArrayExpression<PLUGINTYPE> ChildArray<PLUGINTYPE>()
        {
            validateTypeIsArray<PLUGINTYPE>();

            string propertyName = findPropertyName<PLUGINTYPE>();
            return ChildArray<PLUGINTYPE>(propertyName);
        }

        public ChildArrayExpression<PLUGINTYPE> ChildArray<PLUGINTYPE>(string propertyName)
        {
            validateTypeIsArray<PLUGINTYPE>();

            ChildArrayExpression<PLUGINTYPE> expression =
                new ChildArrayExpression<PLUGINTYPE>(this, _memento, propertyName);
            addChildExpression(expression);

            return expression;
        }

        private static void validateTypeIsArray<PLUGINTYPE>()
        {
            if (!typeof (PLUGINTYPE).IsArray)
            {
                throw new StructureMapException(307);
            }
        }

        #region Nested type: InstanceTypeExpression

        /// <summary>
        /// Helper class to capture the actual concrete type of an Instance
        /// </summary>
        public class InstanceTypeExpression
        {
            private readonly InstanceExpression _parent;

            internal InstanceTypeExpression(InstanceExpression parent)
            {
                _parent = parent;
            }

            /// <summary>
            /// Use type T for the concrete type of an instance
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public InstanceExpression UsingConcreteType<T>()
            {
                _parent._pluggedType = typeof (T);
                return _parent;
            }

            /// <summary>
            /// Use a named Plugin type denoted by a [Pluggable("Key")] attribute
            /// </summary>
            /// <param name="concreteKey"></param>
            /// <returns></returns>
            public InstanceExpression UsingConcreteTypeNamed(string concreteKey)
            {
                _parent._memento.ConcreteKey = concreteKey;
                return _parent;
            }
        }

        #endregion
    }
}