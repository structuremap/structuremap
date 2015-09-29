using System;
using System.Linq;
using StructureMap.Configuration.DSL;
using StructureMap.Graph.Scanning;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public class DefaultConventionScanner : ConfigurableRegistrationConvention
    {
        public override void ScanTypes(TypeSet types, Registry registry)
        {
            types.FindTypes(TypeClassification.Concretes).Where(type => type.HasConstructors()).Each(type =>
            {
                var pluginType = FindPluginType(type);
                if (pluginType != null)
                {
                    registry.AddType(pluginType, type);
                    ConfigureFamily(registry.For(pluginType));
                }
            });
        }

        public virtual Type FindPluginType(Type concreteType)
        {
            var interfaceName = "I" + concreteType.Name;
            return concreteType.GetInterfaces().FirstOrDefault(t => t.Name == interfaceName);
        }
    }
}