using System;
using System.Collections.Generic;
using System.Text;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

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
            if (pluginType != null && Constructor.HasConstructors(type))
            {
                registry.ForRequestedType(pluginType).AddInstance(new ConfiguredInstance(type));
            }
        }

        public virtual Type FindPluginType(Type concreteType)
        {
            string interfaceName = "I" + concreteType.Name;
            Type[] interfaces = concreteType.GetInterfaces();
            return Array.Find(interfaces, t => t.Name == interfaceName);
        }
    }
}
