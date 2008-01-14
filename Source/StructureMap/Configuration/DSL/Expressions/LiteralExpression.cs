using System;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL.Expressions
{
    /// <summary>
    /// Small helper class to represent an object to be plugged into a PluginType as is
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LiteralExpression<T> : MementoBuilder<LiteralExpression<T>>
    {
        private readonly T _target;
        private LiteralMemento _memento;

        public LiteralExpression(T target) : base(typeof (T))
        {
            _target = target;
        }


        protected override InstanceMemento memento
        {
            get { return _memento; }
        }

        protected override LiteralExpression<T> thisInstance
        {
            get { return this; }
        }

        protected override void configureMemento(PluginFamily family)
        {
            _memento.Instance = _target;
        }

        protected override void validate()
        {
        }

        protected override void buildMemento()
        {
            _memento = new LiteralMemento(null);
        }

        public override void ValidatePluggability(Type pluginType)
        {
            ExpressionValidator.ValidatePluggabilityOf(_target.GetType()).IntoPluginType(pluginType);
        }
    }
}