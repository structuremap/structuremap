using System;
using System.Collections.Generic;

namespace StructureMap.Graph
{
    public class GenericsPluginGraph
    {
        private Dictionary<Type, PluginFamily> _families;

        public GenericsPluginGraph()
        {
            _families = new Dictionary<Type, PluginFamily>();
        }

        public static bool CanBeCast(Type pluginType, Type pluggedType)
        {
            //bool isGenericComparison = pluginType.IsGenericType && pluggedType.IsGenericType;
            //if (!isGenericComparison)
            //{
            //    return false;
            //}

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

        public PluginFamily FindGenericFamily(string pluginTypeName)
        {
            foreach (KeyValuePair<Type, PluginFamily> pair in _families)
            {
                if (new TypePath(pair.Key).Matches(pluginTypeName))
                {
                    return pair.Value;
                }
            }

            return null;
        }

        public void AddFamily(PluginFamily family)
        {
            _families.Add(family.PluginType, family);
        }


        public PluginFamily CreateTemplatedFamily(Type templatedType)
        {
            Type basicType = templatedType.GetGenericTypeDefinition();

            if (_families.ContainsKey(basicType))
            {
                PluginFamily basicFamily = _families[basicType];
                Type[] templatedParameterTypes = templatedType.GetGenericArguments();

                return basicFamily.CreateTemplatedClone(templatedParameterTypes);
            }
            else
            {
                return null;
            }
        }

        public PluginFamily CreateTemplatedFamily(string pluginTypeName)
        {
            Type type = Type.GetType(pluginTypeName, true);

            if (!type.IsGenericType)
            {
                return null;
            }

            return CreateTemplatedFamily(type);
        }
    }
}