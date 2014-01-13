using System;
using System.Runtime.Serialization;

namespace StructureMap.Building
{
    [Serializable]
    public class StructureMapConfigurationException : StructureMapException
    {
        protected StructureMapConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public StructureMapConfigurationException(string message) : base(message)
        {
        }

        public StructureMapConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}