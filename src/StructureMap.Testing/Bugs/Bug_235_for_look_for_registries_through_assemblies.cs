using System.Linq;
using NestedLibrary;
using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_235_for_look_for_registries_through_assemblies
    {
        [Test]
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