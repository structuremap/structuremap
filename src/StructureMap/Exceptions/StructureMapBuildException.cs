using System;

namespace StructureMap.Building
{
    public class StructureMapBuildException : StructureMapException
    {
        public StructureMapBuildException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public StructureMapBuildException(string message) : base(message)
        {
        }
    }

    public class StructureMapInterceptorException : StructureMapBuildException
    {
        public StructureMapInterceptorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public StructureMapInterceptorException(string message) : base(message)
        {
        }
    }
}