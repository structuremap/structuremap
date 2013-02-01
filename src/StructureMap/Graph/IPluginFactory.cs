using System;

namespace StructureMap.Graph
{
    public interface IPluginFactory
    {
        Plugin PluginFor(Type pluginType, string name);
    }
}