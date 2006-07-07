using System;

namespace StructureMap.Configuration.Tokens.Properties
{
	[Serializable]
	public class Property : GraphObject, IProperty
	{
		private readonly PropertyDefinition _definition;

		public Property(PropertyDefinition definition)
		{
			_definition = definition;
		}

		public virtual string PropertyName
		{
			get
			{
				return _definition.PropertyName;
			}
		}

		public string PropertyType
		{
			get
			{
				return _definition.PropertyType;
			}
		}

		public PropertyDefinition Definition
		{
			get { return _definition; }
		}

		public virtual void Validate(IInstanceValidator validator)
		{
			// no-op
		}

		protected override string key
		{
			get { return this.PropertyName; }
		}
	}
}
