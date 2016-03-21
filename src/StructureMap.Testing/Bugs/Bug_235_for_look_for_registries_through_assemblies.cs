using NestedLibrary;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_235_for_look_for_registries_through_assemblies
    {
        [Fact]
        public void look_for_registries_fires_assembly_scanning_in_child()
        {
            var container = new Container(x =>
            {
                x.Scan(s =>
                {
                    s.AssemblyContainingType<ITeam>();
                    s.LookForRegistries();
                });
            });

            container.Model.For<ITeam>().Instances.Select(x => x.ReturnedType.Name)
                .OrderBy(x => x)
                .ShouldHaveTheSameElementsAs("Broncos", "Chargers", "Chiefs", "Raiders");
        }
    }
}