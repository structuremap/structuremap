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
			this.InstanceKey = this.ConcreteKey = scope.ToString();			
		}

		public InterceptorInstanceToken(string pluginTypeName, PluginGraphReport report, InstanceMemento memento) : base(pluginTypeName, report, memento)
		{
		}

		public override void AcceptVisitor(IConfigurationVisitor visitor)
		{
			visitor.HandleInterceptor(this);
		}

		protected override string key
		{
			get { return string.Empty; }
		}
	}
}
