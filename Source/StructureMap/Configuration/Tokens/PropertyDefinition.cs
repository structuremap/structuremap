using System;
using StructureMap.Configuration.Tokens.Properties;

namespace StructureMap.Configuration.Tokens
{
    public enum PropertyDefinitionType
    {
        Constructor,
        Setter
    }

    public enum ArgumentType
    {
        Primitive,
        Enumeration,
        Child,
        ChildArray
    }

    [Serializable]
    public class PropertyDefinition : GraphObject
    {
        private string _propertyName;
        private Type _propertyType;
        private PropertyDefinitionType _definitionType;
        private string[] _enumerationValues;
        private ArgumentType _argumentType;


        public PropertyDefinition() : base()
        {
        }

        public PropertyDefinition(string propertyName, Type propertyType, PropertyDefinitionType definitionType,
                                  ArgumentType argumentType) : this()
        {
            _propertyName = propertyName;
            _propertyType = propertyType;
            _definitionType = definitionType;
            _argumentType = argumentType;
        }

        public PropertyDefinition(string propertyName, PropertyDefinitionType definitionType, ArgumentType argumentType)
            : this()
        {
            _propertyName = propertyName;
            _propertyType = null;
            _definitionType = definitionType;
            _argumentType = argumentType;
        }

        public PropertyDefinitionType DefinitionType
        {
            get { return _definitionType; }
            set { _definitionType = value; }
        }

        public Type PropertyType
        {
            get { return _propertyType; }
            set { _propertyType = value; }
        }

        public string PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }

        public string[] EnumerationValues
        {
            get { return _enumerationValues; }
            set { _enumerationValues = value; }
        }

        public ArgumentType ArgumentType
        {
            get { return _argumentType; }
            set { _argumentType = value; }
        }

        public override string ToString()
        {
            return string.Format("Property:  {0}, Type:  {1}", PropertyName, PropertyType);
        }

        public override bool Equals(object obj)
        {
            PropertyDefinition peer = obj as PropertyDefinition;
            if (peer == null)
            {
                return false;
            }

            return PropertyName == peer.PropertyName &&
                   PropertyType == peer.PropertyType &&
                   DefinitionType == peer.DefinitionType &&
                   ArgumentType == peer.ArgumentType;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Property CreateProperty(InstanceMemento memento, PluginGraphReport report)
        {
            switch (ArgumentType)
            {
                case ArgumentType.Primitive:
                    return new PrimitiveProperty(this, memento);

                case ArgumentType.Enumeration:
                    return new EnumerationProperty(this, memento);

                case ArgumentType.Child:
                    return new ChildProperty(this, memento, report);

                case ArgumentType.ChildArray:
                    return new ChildArrayProperty(this, memento, report);

                default:
                    throw new NotImplementedException("Not supporting anything but primitives and enumerations yet");
            }
        }

        public override void AcceptVisitor(IConfigurationVisitor visitor)
        {
            visitor.HandlePropertyDefinition(this);
        }

        protected override string key
        {
            get { return PropertyName; }
        }
    }
}