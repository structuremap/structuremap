using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StructureMap.Testing
{
    public static class RegistryExtensions
    {
        public static PluginGraph Build(this Registry registry)
        {
            var builder = new PluginGraphBuilder();
            builder.Add(registry);

            return builder.Build();
        }
    }
}