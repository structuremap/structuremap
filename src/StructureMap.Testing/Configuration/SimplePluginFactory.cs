using System;
using StructureMap.Graph;

namespace StructureMap.Testing.Configuration
{
    // TODO -- just return the type
    public class SimplePluginFactory : IPluginFactory
    {
        public Plugin PluginFor(string name)
        {
            return new Plugin(Type.GetType(name));
        }
    }
}