using System;
using System.Linq;
using System.Reflection;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace StructureMap.Pipeline
{
    public class GreediestConstructorSelector : IConstructorSelector
    {
        public ConstructorInfo Find(Type pluggedType, PluginGraph graph)
        {
            return pluggedType
                .GetConstructors()
                .OrderByDescending(x => x.GetParameters().Count())
                .FirstOrDefault();
        }
    }
}