using System;

namespace StructureMap.Exceptions
{
    /// <summary>
    /// Thrown by IProperty classes when an invalid value is applied to 
    /// a property of an InstanceGraph
    /// </summary>
    public class InstancePropertyValueException : ApplicationException
    {
        public InstancePropertyValueException(string msg, Exception ex)
            : base(msg, ex)
        {
        }
    }
}