using System;
using System.Reflection;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    public class Policies
    {
        public readonly SetterRules SetterRules = new SetterRules();
        public readonly ConstructorSelector ConstructorSelector = new ConstructorSelector();

        public bool IsMandatorySetter(PropertyInfo propertyInfo)
        {
            return SetterRules.IsMandatory(propertyInfo);
        }

        public ConstructorInfo SelectConstructor(Type pluggedType)
        {
            return ConstructorSelector.Select(pluggedType);
        }
    }
}