using System;
using System.Reflection;
using StructureMap.Attributes;

namespace StructureMap.Graph
{
    /// <summary>
    /// Represents a PropertyInfo of a Plugin.PluggedType that is filled by Setter Injection
    /// </summary>
    public class SetterProperty : TypeRules
    {
        private readonly PropertyInfo _property;


        public SetterProperty(PropertyInfo property)
        {
            _property = property;
            Attribute att = Attribute.GetCustomAttribute(property, typeof (SetterPropertyAttribute));

            IsMandatory = att != null;
        }

        public PropertyInfo Property
        {
            get { return _property; }
        }

        public string Name
        {
            get { return _property.Name; }
        }

        public bool IsMandatory { get; set; }

        public bool CanBeAutoFilled
        {
            get { return IsAutoFillable(_property.PropertyType); }
        }

        public void Visit(IArgumentVisitor visitor)
        {
            Type propertyType = _property.PropertyType;

            if (IsPrimitive(propertyType)) visitor.PrimitiveSetter(_property, IsMandatory);
            if (IsChild(propertyType)) visitor.ChildSetter(_property, IsMandatory);
            if (IsChildArray(propertyType)) visitor.ChildArraySetter(_property, IsMandatory);
            if (IsEnum(propertyType)) visitor.EnumSetter(_property, IsMandatory);
            if (IsString(propertyType)) visitor.StringSetter(_property, IsMandatory);
        }
    }
}