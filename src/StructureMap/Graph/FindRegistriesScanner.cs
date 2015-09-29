using System.Linq;
using StructureMap.Configuration.DSL;
using StructureMap.Graph.Scanning;

namespace StructureMap.Graph
{
    public class FindRegistriesScanner : IRegistrationConvention
    {
        public void ScanTypes(TypeSet types, Registry registry)
        {
            types.FindTypes(TypeClassification.Closed | TypeClassification.Concretes)
                .Where(Registry.IsPublicRegistry)
                .Each(type => registry.Configure(x => x.ImportRegistry(type)));

        }
    }
}