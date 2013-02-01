using System;

namespace StructureMap.Graph
{
    public interface IPluginFactory
    {
        Plugin PluginFor(string name);
    }
}