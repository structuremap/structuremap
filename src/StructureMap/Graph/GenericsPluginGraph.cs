using System;
using System.Reflection;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public static class GenericsPluginGraph
    {
        public static bool CanBeCast(Type pluginType, Type pluggedType)
        {
            try
            {
                return checkGenericType(pluggedType, pluginType);
            }
            catch (Exception e)
            {
                var message =
                    string.Format("Could not Determine Whether Type '{0}' plugs into Type '{1}'",
                        pluginType.Name,
                        pluggedType.Name);
                throw new ArgumentException(message, e);
            }
        }

        private static bool checkGenericType(Type pluggedType, Type pluginType)
        {
            if (pluginType.GetTypeInfo().IsAssignableFrom(pluggedType.GetTypeInfo())) return true;


// check interfaces
            foreach (var type in pluggedType.GetInterfaces())
            {
                if (!type.GetTypeInfo().IsGenericType)
                {
                    continue;
                }

                if (type.GetGenericTypeDefinition() == pluginType)
                {
                    return true;
                }
            }

            if (pluggedType.GetTypeInfo().BaseType.GetTypeInfo().IsGenericType)
            {
                var baseType = pluggedType.GetTypeInfo().BaseType.GetGenericTypeDefinition();

                if (baseType == pluginType)
                {
                    return true;
                }
                else
                {
                    return CanBeCast(pluginType, baseType);
                }
            }

            return false;
        }
    }
}