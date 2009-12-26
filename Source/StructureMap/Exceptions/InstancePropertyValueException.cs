using System;
using System.Runtime.Serialization;

namespace StructureMap.Exceptions
{
    /// <summary>
    /// Thrown by IProperty classes when an invalid value is applied to 
    /// a property of an InstanceGraph
    /// </summary>
    [Serializable]
    public class InstancePropertyValueException : ApplicationException
    {
        public InstancePropertyValueException(string msg, Exception ex)
            : base(msg, ex)
        {
        }

        protected InstancePropertyValueException(SerializationInfo info, StreamingContext context)
            :
                base(info, context)
        {
        }
    }
}