using System;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public class InstanceExpression : MementoBuilder<InstanceExpression>
    {
        private Type _pluggedType;
        private MemoryInstanceMemento _memento;

        public InstanceExpression(Type pluginType) : base(pluginType)
        {
        }


        internal Type PluggedType
        {
            get { return _pluggedType; }
        }

        protected override void buildMemento()
        {
            _memento = new MemoryInstanceMemento();
        }


        protected override InstanceMemento memento
        {
            get { return _memento; }
        }

        protected override InstanceExpression thisInstance
        {
            get { return this; }
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


        public PropertyExpression WithProperty(string propertyName)
        {
            return new PropertyExpression(this, _memento, propertyName);
        }


        public ChildInstanceExpression Child<T>(string propertyName)
        {
            ChildInstanceExpression child = new ChildInstanceExpression(this, _memento, propertyName);
            addChildExpression(child);
            child.ChildType = typeof (T);

            return child;
        }

        public ChildInstanceExpression Child<T>()
        {
            string propertyName = findPropertyName<T>();

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new StructureMapException(305, TypePath.GetAssemblyQualifiedName(typeof (T)));
            }

            ChildInstanceExpression child = new ChildInstanceExpression(this, _memento, propertyName);
            addChildExpression(child);
            child.ChildType = typeof (T);
            return child;
        }

        private string findPropertyName<T>()
        {
            Plugin plugin = Plugin.CreateImplicitPlugin(_pluggedType);
            return plugin.FindFirstConstructorArgumentOfType<T>();
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


        public class InstanceTypeExpression
        {
            private readonly InstanceExpression _parent;

            internal InstanceTypeExpression(InstanceExpression parent)
            {
                _parent = parent;
            }

            public InstanceExpression UsingConcreteType<T>()
            {
                _parent._pluggedType = typeof (T);
                return _parent;
            }

            public InstanceExpression UsingConcreteTypeNamed(string concreteKey)
            {
                _parent._memento.ConcreteKey = concreteKey;
                return _parent;
            }
        }
    }
}