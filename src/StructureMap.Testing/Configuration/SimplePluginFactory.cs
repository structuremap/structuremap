using System;
using StructureMap.Graph;

namespace StructureMap.Testing.Configuration
{
    public class SimplePluginFactory : IPluginFactory
    {
        public Plugin PluginFor(string name)
        {
            return PluginCache.GetPlugin(Type.GetType(name));
        }
    }
}