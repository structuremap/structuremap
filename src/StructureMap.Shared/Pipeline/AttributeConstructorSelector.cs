using System;
using System.Reflection;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public class AttributeConstructorSelector : IConstructorSelector
    {
        public ConstructorInfo Find(Type pluggedType, PluginGraph graph)
        {
            return DefaultConstructorAttribute.GetConstructor(pluggedType);
        }
    }
}