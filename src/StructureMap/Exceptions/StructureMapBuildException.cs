using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

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
        public StructureMapInterceptorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public StructureMapInterceptorException(string message) : base(message)
        {
        }
    }
}