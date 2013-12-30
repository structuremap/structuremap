using System;
using System.Collections;
using System.Reflection;

namespace StructureMap.Pipeline
{
    public class AttributeConstructorSelector : IConstructorSelector
    {
        public ConstructorInfo Find(Type pluggedType)
        {
            return DefaultConstructorAttribute.GetConstructor(pluggedType);
        }
    }
}