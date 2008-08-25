using System;

namespace StructureMap.Exceptions
{
    [Serializable]
    public class StructureMapConfigurationException : ApplicationException
    {
        public StructureMapConfigurationException(string message) : base(message)
        {
        }
    }
}
