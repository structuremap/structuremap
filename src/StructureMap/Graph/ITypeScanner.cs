using System;
using StructureMap.Configuration.DSL;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    [Obsolete("Favor the new IRegistrationConvention")]
    public interface ITypeScanner
    {
        void Process(Type type, PluginGraph graph);
    }

    public class DefaultConventionScanner : ConfigurableRegistrationConvention
    {
        public override void Process(Type type, Registry registry)
        {
            if (!type.IsConcrete()) return;

            Type pluginType = FindPluginType(type);
            if (pluginType != null && Constructor.HasConstructors(type))
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

    public class GenericConnectionScanner : ConfigurableRegistrationConvention
    {
        private readonly Type _openType;

        public GenericConnectionScanner(Type openType)
        {
            _openType = openType;

            if (!_openType.IsOpenGeneric())
            {
                throw new ApplicationException("This scanning convention can only be used with open generic types");
            }
        }

        public override void Process(Type type, Registry registry)
        {
            var interfaceTypes = type.FindInterfacesThatClose(_openType);
            foreach (var interfaceType in interfaceTypes)
            {
                var family = registry.For(interfaceType);
                ConfigureFamily(family);
                family.Add(type);
            }
        }
    }
}