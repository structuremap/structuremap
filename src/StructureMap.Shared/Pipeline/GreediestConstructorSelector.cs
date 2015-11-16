using System;
using System.Linq;
using System.Reflection;
using StructureMap.TypeRules;

namespace StructureMap.Pipeline
{
    public class GreediestConstructorSelector : IConstructorSelector
    {
        public ConstructorInfo Find(Type pluggedType)
        {
            return pluggedType
                .GetConstructors()
                .OrderByDescending(x => x.GetParameters().Count())
                .FirstOrDefault();
        }
    }
}