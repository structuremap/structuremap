using System;
using System.Runtime.Serialization;

namespace StructureMap
{
    [Serializable]
    public class StructureMapConfigurationException : StructureMapException
    {
        protected StructureMapConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public StructureMapConfigurationException(string message, params object[] parameters) : base(message.ToFormat(parameters))
        {
        }

        public StructureMapConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}