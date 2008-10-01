using System;
using StructureMap.Configuration.DSL;

namespace StructureMap.Graph
{
    public class FindRegistriesScanner : ITypeScanner
    {
        public void Process(Type type, PluginGraph graph)
        {
            if (!Registry.IsPublicRegistry(type)) return;

            foreach (var previous in graph.Registries)
            {
                if (previous.GetType().Equals(type))
                {
                    return;
                }
            }

            Registry registry = (Registry)Activator.CreateInstance(type);
            registry.ConfigurePluginGraph(graph);
        }
    }
}