using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public class InstanceExpression : IExpression
    {
        private readonly Type _pluginType;
        private Type _pluggedType;
        private string _propertyName;
        private MemoryInstanceMemento _memento;
        private List<IExpression> _children = new List<IExpression>();
        private PrototypeMemento _prototypeMemento;


        public InstanceExpression(Type pluginType)
        {
            _pluginType = pluginType;
            _memento = new MemoryInstanceMemento();
            _memento.InstanceKey = Guid.NewGuid().ToString();
        }

        void IExpression.Configure(PluginGraph graph)
        {
            if (_prototypeMemento == null && _pluggedType == null && string.IsNullOrEmpty(_memento.ConcreteKey))
            {
                throw new StructureMapException(301, _memento.InstanceKey, TypePath.GetAssemblyQualifiedName(_pluginType));
            }

            PluginFamily family = graph.LocateOrCreateFamilyForType(_pluginType);

            if (_prototypeMemento == null)
            {
                Plugin plugin = _pluggedType == null
                                    ? family.Plugins[_memento.ConcreteKey]
                                    : family.Plugins.FindOrCreate(_pluggedType);

                _memento.ConcreteKey = plugin.ConcreteKey;
                family.Source.AddExternalMemento(_memento);                
            }
            else
            {
                _prototypeMemento.InstanceKey = _memento.InstanceKey;
                family.Source.AddExternalMemento(_prototypeMemento);
            }


        }


        public IExpression[] ChildExpressions
        {
            get { return _children.ToArray(); }
        }


        public InstanceExpression WithName(string instanceKey)
        {
            _memento.InstanceKey = instanceKey;
            return this;
        }


        public string InstanceKey
        {
            get { return _memento.InstanceKey; }
            set { _memento.InstanceKey = value; }
        }

        internal Type PluginType
        {
            get { return _pluginType; }
        }

        public InstanceExpression UsingConcreteType<T>()
        {
            _pluggedType = typeof (T);
            return this;
        }

        public PropertyExpression WithProperty(string propertyName)
        {
            return new PropertyExpression(this, _memento, propertyName);
        }




        public void UsePrototype(ICloneable cloneable)
        {
            _prototypeMemento = new PrototypeMemento(_memento.InstanceKey, cloneable);
        }


        public InstanceExpression UsingConcreteTypeNamed(string concreteKey)
        {
            _memento.ConcreteKey = concreteKey;
            return this;
        }

        public ChildInstanceExpression Child(string propertyName)
        {
            ChildInstanceExpression child = new ChildInstanceExpression(this,_memento, propertyName);
            _children.Add(child);

            return child;
        }

        public ChildInstanceExpression Child<T>()
        {
            // TODO -- what if the property can't be found
            string propertyName = findPropertyName<T>();
            ChildInstanceExpression child = Child(propertyName);
            child.ChildType = typeof (T);
            return child;
        }

        private string findPropertyName<T>()
        {
            Plugin plugin = Plugin.CreateImplicitPlugin(_pluggedType);
            return plugin.FindFirstConstructorArgumentOfType<T>();
        }


        
    }
}
