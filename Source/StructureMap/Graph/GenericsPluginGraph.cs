using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StructureMap.Graph
{
    public class GenericsPluginGraph
    {
        public static bool CanBeCast(Type pluginType, Type pluggedType)
        {
            bool isGenericComparison = pluginType.IsGenericType && pluggedType.IsGenericType;
            if (!isGenericComparison)
            {
                return false;
            }

            // check interfaces
            foreach (Type type in pluggedType.GetInterfaces())
            {
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

        private Dictionary<Type, PluginFamily> _families;
        
        public GenericsPluginGraph()
        {
            _families = new Dictionary<Type, PluginFamily>();
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
    }
    
    
}
