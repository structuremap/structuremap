using System;

namespace StructureMap.Attributes
{
    /// <summary>
    /// Marks a Property in a Pluggable class as filled by setter injection 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SetterPropertyAttribute : Attribute
    {
    }
}