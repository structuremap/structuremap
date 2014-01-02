using System;
using StructureMap.Configuration.DSL;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public class DefaultConventionScanner : ConfigurableRegistrationConvention
    {
        public override void Process(Type type, Registry registry)
        {
            if (!type.IsConcrete()) return;

            Type pluginType = FindPluginType(type);
            if (pluginType != null && type.HasConstructors())
            {
                registry.AddType(pluginType, type);
                ConfigureFamily(registry.For(pluginType));
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