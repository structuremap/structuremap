using System;
using StructureMap.Configuration.DSL;

namespace StructureMap.Graph
{
    public class PluggableAttributeScanner : IRegistrationConvention
    {
        public void Process(Type type, Registry registry)
        {
            if (PluggableAttribute.MarkedAsPluggable(type))
            {
                registry.AddType(type);
            }
        }
    }
}