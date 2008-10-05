using System;

namespace StructureMap.Graph
{
    public interface ITypeScanner
    {
        void Process(Type type, PluginGraph graph);
    }

    public class DefaultConventionScanner : TypeRules, ITypeScanner
    {
        #region ITypeScanner Members

        public void Process(Type type, PluginGraph graph)
        {
            if (!IsConcrete(type)) return;

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
}