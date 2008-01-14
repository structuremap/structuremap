using System;
using StructureMap.Configuration.Mementos;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL.Expressions
{
    public class UserControlExpression : MementoBuilder<UserControlExpression>
    {
        private UserControlMemento _memento;

        public UserControlExpression(Type pluginType, string url) : base(pluginType)
        {
            _memento.Url = url;
        }

        protected override InstanceMemento memento
        {
            get { return _memento; }
        }

        protected override UserControlExpression thisInstance
        {
            get { return this; }
        }

        protected override void configureMemento(PluginFamily family)
        {
            // no-op
        }

        protected override void validate()
        {
            // no-op
        }

        protected override void buildMemento()
        {
            _memento = new UserControlMemento();
        }

        public override void ValidatePluggability(Type pluginType)
        {
            // no-op
        }
    }
}