using System;
using System.Reflection;
using StructureMap.Attributes;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    /// <summary>
    /// Represents a PropertyInfo of a Plugin.PluggedType that is filled by Setter Injection
    /// </summary>
    [Obsolete("Should not be necessary going forward")]
    public class SetterProperty
    {
        private readonly PropertyInfo _property;


        public SetterProperty(PropertyInfo property)
        {
            _property = property;
        }

        public PropertyInfo Property { get { return _property; } }

        public string Name { get { return _property.Name; } }

        public bool IsMandatory { get; set; }

        public bool CanBeAutoFilled { get { return _property.PropertyType.IsAutoFillable(); } }

        public void Visit(IArgumentVisitor visitor)
        {
            Type propertyType = _property.PropertyType;

            // Ignore indexer properties
            if (_property.GetIndexParameters().Length > 0) return;

            if (propertyType.IsPrimitive()) visitor.PrimitiveSetter(_property, IsMandatory);
            if (propertyType.IsChild()) visitor.ChildSetter(_property, IsMandatory);
            if (propertyType.IsChildArray()) visitor.ChildArraySetter(_property, IsMandatory);
            if (propertyType.IsEnum) visitor.EnumSetter(_property, IsMandatory);
            if (propertyType.IsString()) visitor.StringSetter(_property, IsMandatory);
        }
    }
}