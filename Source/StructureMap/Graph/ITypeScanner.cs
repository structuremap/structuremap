using System;
using System.Collections.Generic;
using System.Text;
using StructureMap.Configuration.DSL;

namespace StructureMap.Graph
{
    public interface ITypeScanner
    {
        void Process(Type type, Registry registry);
    }

    public class DefaultConventionScanner : TypeRules, ITypeScanner
    {
        public void Process(Type type, Registry registry)
        {
            if (!IsConcrete(type)) return;

            Type pluginType = FindPluginType(type);
            if (pluginType != null)
            {
                registry.ForRequestedType(pluginType).TheDefaultIsConcreteType(type);
            }
        }

        public static Type FindPluginType(Type concreteType)
        {
            string interfaceName = "I" + concreteType.Name;
            Type[] interfaces = concreteType.GetInterfaces();
            return Array.Find(interfaces, delegate(Type t)
            {
                return t.Name == interfaceName;
            });
        }
    }
}
