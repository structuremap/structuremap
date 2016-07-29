using System;
using System.Linq;
using System.Reflection;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace StructureMap.Pipeline
{
    public class GreediestConstructorSelector : IConstructorSelector
    {
        public ConstructorInfo Find(Type pluggedType, DependencyCollection dependencies, PluginGraph graph)
        {
            return pluggedType
                .GetConstructors()
                .Where(x => !HasMissingPrimitives(x, dependencies))
                .OrderByDescending(x => x.GetParameters().Count())
                .FirstOrDefault();
        }

        public static bool HasMissingPrimitives(ConstructorInfo ctor, DependencyCollection dependencies)
        {
            return ctor
                .GetParameters()
                .Where(x => x.ParameterType.IsSimple())
                .Any(param => dependencies.FindByTypeOrName(param.ParameterType, param.Name) == null);
        }
    }
}