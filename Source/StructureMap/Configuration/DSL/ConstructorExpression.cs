using System;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public class ConstructorExpression<PLUGINTYPE> : MementoBuilder<ConstructorExpression<PLUGINTYPE>>
    {
        private ConstructorMemento<PLUGINTYPE> _memento;

        public ConstructorExpression(BuildObjectDelegate<PLUGINTYPE> builder)
            : base(typeof (PLUGINTYPE))
        {
            _memento.Builder = builder;
        }


        protected override InstanceMemento memento
        {
            get { return _memento; }
        }

        protected override ConstructorExpression<PLUGINTYPE> thisInstance
        {
            get { return this; }
        }

        protected override void configureMemento(PluginFamily family)
        {
        }

        protected override void validate()
        {
        }

        protected override void buildMemento()
        {
            _memento = new ConstructorMemento<PLUGINTYPE>();
        }

        public override void ValidatePluggability(Type pluginType)
        {
            if (!pluginType.Equals(typeof (PLUGINTYPE)))
            {
                throw new StructureMapException(306,
                                                typeof (PLUGINTYPE).FullName, pluginType.FullName);
            }
        }
    }
}