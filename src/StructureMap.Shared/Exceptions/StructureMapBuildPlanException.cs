using System;

namespace StructureMap
{
    public class StructureMapBuildPlanException : StructureMapException
    {
        public StructureMapBuildPlanException(string message) : base(message)
        {
        }

        public StructureMapBuildPlanException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}