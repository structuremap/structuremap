using System;

namespace StructureMap.Configuration.Tokens
{
	[Serializable]
	public class MementoSourceInstanceToken : InstanceToken
	{
		public MementoSourceInstanceToken() : base()
		{
		}

		public MementoSourceInstanceToken(string pluginTypeName, PluginGraphReport report, InstanceMemento memento) : base(pluginTypeName, report, memento)
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
