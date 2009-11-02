using System;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public interface ITypeScanner
    {
        void Process(Type type, PluginGraph graph);
    }

    public class DefaultConventionScanner : ITypeScanner
    {
        #region ITypeScanner Members

        public void Process(Type type, PluginGraph graph)
        {
            if (!type.IsConcrete()) return;

            Type pluginType = FindPluginType(type);
            if (pluginType != null && Constructor.HasConstructors(type))
            {
                graph.AddType(pluginType, type);
            }
        }

        #endregion

        public virtual Type FindPluginType(Type concreteType)
        {
            string interfaceName = "I" + concreteType.Name;
            Type[] interfaces = concreteType.GetInterfaces();
            return Array.Find(interfaces, t => t.Name == interfaceName);
        }
    }

    public class GenericConnectionScanner : ITypeScanner
    {
        private readonly Type _openType;

        public GenericConnectionScanner(Type openType)
        {
            _openType = openType;

            if (!_openType.IsGeneric())
            {
                throw new ApplicationException("This scanning convention can only be used with open generic types");
            }
        }

        public void Process(Type type, PluginGraph graph)
        {
            Type interfaceType = type.FindInterfaceThatCloses(_openType);
            if (interfaceType != null)
            {
                graph.AddType(interfaceType, type);
            }
        }
    }
}