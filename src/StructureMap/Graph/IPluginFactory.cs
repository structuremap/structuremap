using System;

namespace StructureMap.Graph
{
    public interface IPluginFactory
    {
        // TODO -- change this to just return the Type
        Plugin PluginFor(string name);
    }
}