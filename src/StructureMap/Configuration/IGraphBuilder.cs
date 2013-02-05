using System;
using StructureMap.Graph;

namespace StructureMap.Configuration
{
    public interface IGraphBuilder
    {
        PluginGraph PluginGraph { get; }
        void AddRegistry(string registryTypeName);
        void ConfigureFamily(TypePath pluginTypePath, Action<PluginFamily> action);
    }
}