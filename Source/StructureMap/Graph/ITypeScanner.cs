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

    public interface IRegistrationConvention
    {
        void Process(Type type, Registry registry);
    }

    public class DefaultConventionScanner : IRegistrationConvention
    {
        public void Process(Type type, Registry registry)
        {
            if (!type.IsConcrete()) return;

            Type pluginType = FindPluginType(type);
            if (pluginType != null && Constructor.HasConstructors(type))
            {
                registry.For(pluginType).Add(type);
            }
        }

        public virtual Type FindPluginType(Type concreteType)
        {
            string interfaceName = "I" + concreteType.Name;
            Type[] interfaces = concreteType.GetInterfaces();
            return Array.Find(interfaces, t => t.Name == interfaceName);
        }
    }

    public class GenericConnectionScanner : IRegistrationConvention
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

        public void Process(Type type, Registry registry)
        {
            Type interfaceType = type.FindInterfaceThatCloses(_openType);
            if (interfaceType != null)
            {
                registry.For(interfaceType).Add(type);
            }
        }
    }
}