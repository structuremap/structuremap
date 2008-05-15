using System;
using System.Reflection;

namespace StructureMap.Graph
{
    /// <summary>
    /// Represents a PropertyInfo of a Plugin.PluggedType that is filled by Setter Injection
    /// </summary>
    public class SetterProperty : TypeRules
    {
        private readonly PropertyInfo _property;

        public SetterProperty(PropertyInfo property) : base()
        {
            _property = property;
        }

        public PropertyInfo Property
        {
            get { return _property; }
        }

        public string Name
        {
            get { return _property.Name; }
        }

        public bool CanBeAutoFilled
        {
            get { return IsChild(_property.PropertyType); }
        }

        public void Visit(IArgumentVisitor visitor)
        {
            Type propertyType = _property.PropertyType;

            if (IsPrimitive(propertyType)) visitor.PrimitiveSetter(_property);
            if (IsChild(propertyType)) visitor.ChildSetter(_property);
            if (IsChildArray(propertyType)) visitor.ChildArraySetter(_property);
            if (IsEnum(propertyType)) visitor.EnumSetter(_property);
            if (IsString(propertyType)) visitor.StringSetter(_property);
        }
    }
}