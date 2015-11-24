using System;
using System.Reflection;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public class AttributeConstructorSelector : IConstructorSelector
    {
        public ConstructorInfo Find(Type pluggedType, DependencyCollection dependencies, PluginGraph graph)
        {
            return DefaultConstructorAttribute.GetConstructor(pluggedType);
        }
    }
}