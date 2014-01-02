using System;

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
                throw new ApplicationException(message, e);
            }
        }

        private static bool checkGenericType(Type pluggedType, Type pluginType)
        {
            if (pluginType.IsAssignableFrom(pluggedType)) return true;


// check interfaces
            foreach (var type in pluggedType.GetInterfaces())
            {
                if (!type.IsGenericType)
                {
                    continue;
                }

                if (type.GetGenericTypeDefinition().Equals(pluginType))
                {
                    return true;
                }
            }

            if (pluggedType.BaseType.IsGenericType)
            {
                var baseType = pluggedType.BaseType.GetGenericTypeDefinition();

                if (baseType.Equals(pluginType))
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