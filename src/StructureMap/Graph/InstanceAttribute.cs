using System;
using StructureMap.Pipeline;

namespace StructureMap.Graph
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class InstanceAttribute : Attribute
    {
        public abstract void Alter(ConstructorInstance instance);
    }
}