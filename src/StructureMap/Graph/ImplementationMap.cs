using System.Linq;
using StructureMap.Configuration.DSL;
using StructureMap.Graph.Scanning;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public class ImplementationMap : ConfigurableRegistrationConvention
    {
        public override void ScanTypes(TypeSet types, Registry registry)
        {
            var interfaces = types.FindTypes(TypeClassification.Interfaces);
            var concretes = types.FindTypes(TypeClassification.Concretes).Where(x => x.HasConstructors()).ToArray();

            interfaces.Each(@interface =>
            {
                var implementors = concretes.Where(x => x.CanBeCastTo(@interface)).ToArray();
                if (implementors.Count() == 1)
                {
                    registry.AddType(@interface, implementors.Single());
                    ConfigureFamily(registry.For(@interface));
                }
            });
        }
    }
}