using System;
using System.Runtime.Serialization;

namespace StructureMap.Exceptions
{
    [Serializable]
    public class StructureMapConfigurationException : ApplicationException
    {
        public StructureMapConfigurationException(string message)
            : base(message)
        {
        }


        protected StructureMapConfigurationException(SerializationInfo info, StreamingContext context)
            :
                base(info, context)
        {
        }
    }
}