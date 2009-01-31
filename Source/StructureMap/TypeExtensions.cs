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

        public static bool IsSimple(this Type type)
        {
            return new TypeRules().IsSimple(type);
        }

        public static bool IsConcrete(this Type type)
        {
            return new TypeRules().IsConcrete(type);
        }

        public static bool IsGeneric(this Type type)
        {
            return type.IsGenericTypeDefinition || type.ContainsGenericParameters;
        }

        public static bool CanBeCastTo(this Type pluggedType, Type pluginType)
        {
            return TypeRules.CanBeCast(pluginType, pluggedType);
        }

        public static bool IsConcreteAndAssignableTo(this Type pluggedType, Type pluginType)
        {
            return pluggedType.IsConcrete() && pluginType.IsAssignableFrom(pluggedType);
        }

        public static bool ImplementsInterfaceTemplate(this Type pluggedType, Type templateType)
        {
            if (!pluggedType.IsConcrete()) return false;

            foreach (var interfaceType in pluggedType.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == templateType)
                {
                    return true;
                }
            }

            return false;
        }

        public static Type FindInterfaceThatCloses(this Type pluggedType, Type templateType)
        {
            if (!pluggedType.IsConcrete()) return null;

            foreach (var interfaceType in pluggedType.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == templateType)
                {
                    return interfaceType;
                }
            }

            return null;
        }

        public static string GetName(this Type type)
        {
            if (type.IsGenericType)
            {
                string[] parameters = Array.ConvertAll(type.GetGenericArguments(), t => t.GetName());
                var parameterList = string.Join(", ", parameters);
                return "{0}<{1}>".ToFormat(type.Name, parameterList);
            }

            return type.Name;
        }

        public static string GetFullName(this Type type)
        {
            if (type.IsGenericType)
            {
                string[] parameters = Array.ConvertAll(type.GetGenericArguments(), t => t.GetName());
                var parameterList = string.Join(", ", parameters);
                return "{0}<{1}>".ToFormat(type.Name, parameterList);
            }

            return type.FullName;
        }

    }
}