using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace StructureMap.Building
{

    [Serializable]
    public class StructureMapBuildException : StructureMapException
    {
        public StructureMapBuildException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public StructureMapBuildException(string message) : base(message)
        {
        }

        protected StructureMapBuildException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}