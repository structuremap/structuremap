using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    public static class TypeExtensions
    {
        public static bool IsInNamespace(this Type type, string nameSpace)
        {
            return type.Namespace.StartsWith(nameSpace);
        }

        public static ReferencedInstance GetReferenceTo(this Type type)
        {
            string key = PluginCache.GetPlugin(type).ConcreteKey;
            return new ReferencedInstance(key);
        }

        public static bool IsConcrete(this Type type)
        {
            return new TypeRules().IsConcrete(type);
        }
    }
}