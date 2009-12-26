using System;
using StructureMap.Configuration.DSL;

namespace StructureMap.Graph
{
    public class FindRegistriesScanner : IRegistrationConvention
    {
        public void Process(Type type, Registry registry)
        {
            if (Registry.IsPublicRegistry(type))
            {
                registry.Configure(x => x.ImportRegistry(type));
            }
        }
    }
}