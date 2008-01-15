using System;
using StructureMap.Attributes;

namespace StructureMap.Configuration.Tokens
{
    [Serializable]
    public class InterceptorInstanceToken : InstanceToken
    {
        public InterceptorInstanceToken() : base()
        {
        }

        public InterceptorInstanceToken(InstanceScope scope) : base()
        {
            InstanceKey = ConcreteKey = scope.ToString();
        }

        public InterceptorInstanceToken(Type pluginType, PluginGraphReport report, InstanceMemento memento)
            : base(pluginType, report, memento)
        {
        }

        protected override string key
        {
            get { return string.Empty; }
        }

        public override void AcceptVisitor(IConfigurationVisitor visitor)
        {
            visitor.HandleInterceptor(this);
        }
    }
}