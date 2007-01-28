using System;

namespace StructureMap.Configuration.Tokens
{
    [Serializable]
    public class MementoSourceInstanceToken : InstanceToken
    {
        public MementoSourceInstanceToken() : base()
        {
        }

        public MementoSourceInstanceToken(Type pluginType, PluginGraphReport report, InstanceMemento memento)
            : base(pluginType, report, memento)
        {
        }

        public override void AcceptVisitor(IConfigurationVisitor visitor)
        {
            visitor.HandleMementoSource(this);
        }

        protected override string key
        {
            get { return string.Empty; }
        }
    }
}