using System;

namespace StructureMap.Configuration.Tokens.Properties
{
	[Serializable]
	public class EnumerationProperty : Property
	{
		private string _value;

		public EnumerationProperty(PropertyDefinition definition, InstanceMemento memento) : base(definition)
		{
			try
			{
				_value = memento.GetProperty(definition.PropertyName);
				checkValue();
			}
			catch (Exception ex)
			{
				Problem problem = new Problem(ConfigurationConstants.MEMENTO_PROPERTY_IS_MISSING, ex);
				LogProblem(problem);
			}
		}

		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		private void checkValue()
		{
			if (Array.IndexOf(this.Definition.EnumerationValues, _value) < 0)
			{
				string message = 
					string.Format("The valid values for the Enumeration {0} are {1}", 
					this.Definition.PropertyType, string.Join(", ", this.Definition.EnumerationValues) ) ;

				Problem problem = new Problem(ConfigurationConstants.INVALID_ENUMERATION_VALUE, message);
				LogProblem(problem);
			}
		}

		public override void AcceptVisitor(IConfigurationVisitor visitor)
		{
			visitor.HandleEnumerationProperty(this);
		}
	}
}
