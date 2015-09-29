using System;
using System.Linq;
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
            var registry = new Registry();
            types.FindTypes(TypeClassification.Closed | TypeClassification.Concretes)
                .Where(Registry.IsPublicRegistry)
                .Each(type => registry.Configure(x => x.ImportRegistry(type)));


            return registry;
        }
    }
}