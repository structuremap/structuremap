using System;

namespace StructureMap.Configuration.Tokens.Properties
{
	[Serializable]
	public class ReferencedChildPropertyMode : IChildPropertyMode
	{
		private readonly ChildProperty _property;

		public ReferencedChildPropertyMode(ChildProperty property)
		{
			_property = property;
		}

		public void Validate(IInstanceValidator validator)
		{
			if (!validator.InstanceExists(_property.PluginType, _property.ReferenceKey))
			{
				string message = string.Format(
					"The configured reference to instance {0} is not configured for PluginType {1}", 
					_property.ReferenceKey, _property.PluginType);

				Problem problem = new Problem(ConfigurationConstants.NO_MATCHING_INSTANCE_CONFIGURED, message);
				_property.LogProblem(problem);
			}
		}

		public void AcceptVisitor(IConfigurationVisitor visitor)
		{
			visitor.HandleReferenceChildProperty(_property);
		}
	}
}
