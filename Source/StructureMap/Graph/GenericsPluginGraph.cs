using System;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace StructureMap.Graph
{
    public class GenericsPluginGraph
    {
        private readonly Dictionary<Type, PluginFamily> _families;

        public GenericsPluginGraph()
        {
            _families = new Dictionary<Type, PluginFamily>();
        }

        public static bool CanBeCast(Type pluginType, Type pluggedType)
        {
            try
            {
                return checkGenericType(pluggedType, pluginType);
            }
            catch (Exception e)
            {
                string message =
                    string.Format("Could not Determine Whether Type '{0}' plugs into Type '{1}'",
                                  pluginType.Name,
                                  pluggedType.Name);
                throw new ApplicationException(message, e);
            }
        }

        private static bool checkGenericType(Type pluggedType, Type pluginType)
        {
// check interfaces
            foreach (Type type in pluggedType.GetInterfaces())
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
                Type baseType = pluggedType.BaseType.GetGenericTypeDefinition();

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

        public void AddFamily(PluginFamily family)
        {
            _families.Add(family.PluginType, family);
        }


        public PluginFamily CreateTemplatedFamily(Type templatedType, ProfileManager profileManager)
        {
            Type basicType = templatedType.GetGenericTypeDefinition();

            if (_families.ContainsKey(basicType))
            {
                PluginFamily basicFamily = _families[basicType];
                Type[] templatedParameterTypes = templatedType.GetGenericArguments();

                profileManager.CopyDefaults(basicType, templatedType);

                return basicFamily.CreateTemplatedClone(templatedParameterTypes);
            }
            else
            {
                return null;
            }
        }
    }
}