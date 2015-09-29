using System;
using StructureMap.Configuration.DSL;
using StructureMap.Graph.Scanning;

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

        public Registry ScanTypes(TypeSet types)
        {
            throw new NotImplementedException();
        }
    }
}