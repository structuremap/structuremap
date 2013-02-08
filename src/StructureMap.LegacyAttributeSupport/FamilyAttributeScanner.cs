using System;
using StructureMap.Configuration.DSL;

namespace StructureMap.Graph
{
    public class FamilyAttributeScanner : IRegistrationConvention
    {
        public void Process(Type type, Registry registry)
        {
            if (PluginFamilyAttribute.MarkedAsPluginFamily(type))
            {
                registry.Configure(x => x.FindFamily(type));
            }
        }
    }
}