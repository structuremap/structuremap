using System;
using StructureMap.Configuration.DSL;

namespace StructureMap.Graph
{
    public class FindRegistriesScanner : ITypeScanner
    {
        #region ITypeScanner Members

        public void Process(Type type, PluginGraph graph)
        {
            if (!Registry.IsPublicRegistry(type)) return;

            foreach (Registry previous in graph.Registries)
            {
                if (previous.GetType().Equals(type))
                {
                    return;
                }
            }

            var registry = (Registry) Activator.CreateInstance(type);
            registry.ConfigurePluginGraph(graph);
        }

        #endregion
    }
}