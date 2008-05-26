using System;
using System.Collections.Generic;
using System.Text;

namespace StructureMap.Exceptions
{
    public class StructureMapConfigurationException : ApplicationException
    {
        public StructureMapConfigurationException(string message) : base(message)
        {
        }
    }
}
