using System;

namespace StructureMap.Configuration.Tokens.Properties
{
	public enum ChildPropertyType
	{
		InlineDefinition,
		Reference,
		Default,
		NotDefined
	}

	[Serializable]
	public class ChildProperty : Property
	{
		public static ChildProperty BuildArrayChild(PropertyDefinition definition, InstanceMemento memento, PluginGraphReport report)
		{
			ChildProperty child = new ChildProperty(definition);
			child.initialize(definition, memento, report);

			return child;
		}


		private ChildPropertyType _childType = ChildPropertyType.NotDefined;
		private InstanceToken _innerInstance;
		private string _referenceKey = string.Empty;
		private IChildPropertyMode _mode;
		private string _pluginTypeName;
		private int _arrayIndex = -1;

		protected ChildProperty(PropertyDefinition definition) : base(definition)
		{
			_mode = new NulloChildPropertyMode(this);
		}

		public ChildProperty(PropertyDefinition definition, InstanceMemento memento, PluginGraphReport report) : this(definition)
		{
			InstanceMemento propertyMemento = memento.GetChildMemento(definition.PropertyName);
			initialize(definition, propertyMemento, report);
		}

		public override string PropertyName
		{
			get
			{
				if (this.ArrayIndex >= 0)
				{
					return base.PropertyName + " #" + this.ArrayIndex;
				}
				return base.PropertyName;
			}
		}

		public int ArrayIndex
		{
			get { return _arrayIndex; }
			set { _arrayIndex = value; }
		}

		private void initialize(PropertyDefinition definition, InstanceMemento propertyMemento, PluginGraphReport report)
		{
			_pluginTypeName = definition.PropertyType;
	
			if (propertyMemento == null)
			{
				// Assume that the reference is to the default of the property's type
				_childType = ChildPropertyType.Default;
				_mode = new DefaultChildPropertyMode(this);
			}
			else if (propertyMemento.IsDefault)
			{
				_childType = ChildPropertyType.Default;
				_mode = new DefaultChildPropertyMode(this);
			}
			else if (propertyMemento.IsReference)
			{
				_childType = ChildPropertyType.Reference;
				_referenceKey = propertyMemento.ReferenceKey;
				_mode = new ReferencedChildPropertyMode(this);
			}
			else
			{
				_childType = ChildPropertyType.InlineDefinition;
				_innerInstance = new InstanceToken(definition.PropertyType, report, propertyMemento);
				_mode = new InlineInstanceChildPropertyMode(this);
			}
		}

		public ChildPropertyType ChildType
		{
			get { return _childType; }
			set { _childType = value; }
		}

		public InstanceToken InnerInstance
		{
			get { return _innerInstance; }
			set { _innerInstance = value; }
		}

		public string ReferenceKey
		{
			get { return _referenceKey; }
			set {_referenceKey = value;}
		}

		public string PluginTypeName
		{
			get { return _pluginTypeName; }
			set { _pluginTypeName = value; }
		}

		public override void Validate(IInstanceValidator validator)
		{
			_mode.Validate(validator);
		}

		public override GraphObject[] Children
		{
			get
			{
				if (_innerInstance == null)
				{
					return new GraphObject[0];
				}
				else
				{
					return new GraphObject[]{_innerInstance};
				}
			}
		}

		public override void AcceptVisitor(IConfigurationVisitor visitor)
		{
			_mode.AcceptVisitor(visitor);
		}
	}
}
