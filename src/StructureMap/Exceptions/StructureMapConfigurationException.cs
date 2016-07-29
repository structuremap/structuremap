using System;

namespace StructureMap
{
    public class StructureMapConfigurationException : StructureMapException
    {
        public StructureMapConfigurationException(string message, params object[] parameters)
            : base(message.ToFormat(parameters))
        {
        }

        public StructureMapConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}