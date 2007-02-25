using System;
using System.Collections.Generic;
using System.Text;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public class ChildInstanceExpression : IExpression
    {
        private readonly InstanceExpression _instance;
        private readonly MemoryInstanceMemento _memento;
        private readonly string _propertyName;
        private Plugin _plugin;
        private Type _childType;
        private List<IExpression> _children = new List<IExpression>();


        public ChildInstanceExpression(InstanceExpression instance, MemoryInstanceMemento memento, string propertyName)
        {
            _instance = instance;
            _memento = memento;
            _propertyName = propertyName;
        }



        public InstanceExpression IsNamedInstance(string instanceKey)
        {
            MemoryInstanceMemento child = MemoryInstanceMemento.CreateReferencedInstanceMemento(instanceKey);
            _memento.AddChild(_propertyName, child);

            return _instance;
        }

        // TODO -- negative case if the concrete type cannot be an implicit instance
        public InstanceExpression IsConcreteType<T>()
        {
            _plugin = Plugin.CreateImplicitPlugin(typeof(T));
            MemoryInstanceMemento child = MemoryInstanceMemento.CreateReferencedInstanceMemento(_plugin.ConcreteKey);
            _memento.AddChild(_propertyName, child);

            return _instance;
        }


        void IExpression.Configure(PluginGraph graph)
        {
            if (_plugin == null || _childType == null)
            {
                return;
            }

            PluginFamily family = graph.LocateOrCreateFamilyForType(_childType);
            family.Plugins.FindOrCreate(_plugin.PluggedType);
        }


        IExpression[] IExpression.ChildExpressions
        {
            get { return _children.ToArray(); }
        }

        internal Type ChildType
        {
            set { _childType = value; }
        }

        public InstanceExpression Is(InstanceExpression child)
        {
            _children.Add(child);
            MemoryInstanceMemento childMemento = MemoryInstanceMemento.CreateReferencedInstanceMemento(child.InstanceKey);
            _memento.AddChild(_propertyName, childMemento);

            return _instance;
        }
    }
}
