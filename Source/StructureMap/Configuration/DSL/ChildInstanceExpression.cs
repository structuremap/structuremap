using System;
using System.Collections.Generic;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    /// <summary>
    /// Part of the Fluent Interface, represents a nonprimitive argument to a 
    /// constructure function
    /// </summary>
    public class ChildInstanceExpression : IExpression
    {
        private readonly InstanceExpression _instance;
        private readonly MemoryInstanceMemento _memento;
        private readonly string _propertyName;
        private Type _childType;
        private List<IExpression> _children = new List<IExpression>();
        private IMementoBuilder _builder;


        public ChildInstanceExpression(InstanceExpression instance, MemoryInstanceMemento memento, string propertyName)
        {
            _instance = instance;
            _memento = memento;
            _propertyName = propertyName;
        }

        public ChildInstanceExpression(InstanceExpression instance, MemoryInstanceMemento memento, string propertyName,
                                       Type childType)
            : this(instance, memento, propertyName)
        {
            _childType = childType;
        }

        /// <summary>
        /// Use a previously configured and named instance for the child
        /// </summary>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public InstanceExpression IsNamedInstance(string instanceKey)
        {
            MemoryInstanceMemento child = MemoryInstanceMemento.CreateReferencedInstanceMemento(instanceKey);
            _memento.AddChild(_propertyName, child);

            return _instance;
        }

        /// <summary>
        /// Start the definition of a child instance by defining the concrete type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public InstanceExpression IsConcreteType<T>()
        {
            Type pluggedType = typeof (T);
            ExpressionValidator.ValidatePluggabilityOf(pluggedType).IntoPluginType(_childType);


            InstanceExpression child = new InstanceExpression(_childType);
            child.TypeExpression().UsingConcreteType<T>();
            _children.Add(child);

            _builder = child;

            return _instance;
        }


        void IExpression.Configure(PluginGraph graph)
        {
            if (_childType == null)
            {
                return;
            }

            PluginFamily family = graph.LocateOrCreateFamilyForType(_childType);
            if (_builder != null)
            {
                InstanceMemento childMemento = _builder.BuildMemento(family);
                _memento.AddChild(_propertyName, childMemento);
            }

            foreach (IExpression child in _children)
            {
                child.Configure(graph);
            }
        }

        internal Type ChildType
        {
            set { _childType = value; }
        }

        /// <summary>
        /// Registers a configured instance to use as the argument to the parent's
        /// constructor
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public InstanceExpression Is(InstanceExpression child)
        {
            if (child.PluggedType != null && _childType != null)
            {
                ExpressionValidator.ValidatePluggabilityOf(child.PluggedType).IntoPluginType(_childType);
            }

            _children.Add(child);
            MemoryInstanceMemento childMemento =
                MemoryInstanceMemento.CreateReferencedInstanceMemento(child.InstanceKey);
            _memento.AddChild(_propertyName, childMemento);

            return _instance;
        }
    }
}