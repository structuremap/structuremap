using System;
using System.Linq;
using System.Reflection;
using StructureMap.Configuration.DSL;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public class DefaultConventionScanner : ConfigurableRegistrationConvention
    {
        public override void Process(Type type, Registry registry)
        {
            if (!type.IsConcrete()) return;

            var pluginType = FindPluginType(type);
            if (pluginType != null && type.HasConstructors())
            {
                registry.AddType(pluginType, type);
                ConfigureFamily(registry.For(pluginType));
            }
        }

        public virtual Type FindPluginType(Type concreteType)
        {
            var interfaceName = "I" + concreteType.Name;
            return concreteType.GetInterfaces().FirstOrDefault(t => t.Name == interfaceName);
        }
    }
}