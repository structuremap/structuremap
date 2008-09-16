using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Testing.Widget2;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class EnumerationTester
    {
        [Test]
        public void BuildClassWithEnumeration()
        {
            PluginGraph graph = new PluginGraph();


            PluginFamily family = graph.FindFamily(typeof (Cow));
            family.AddPlugin(typeof (Cow), "Default");

            Container manager = new Container(graph);

            manager.Configure(r => r.InstanceOf<Cow>().Is.OfConcreteType<Cow>()
                                              .WithName("Angus")
                                              .WithProperty("Name").EqualTo("Bessie")
                                              .WithProperty("Breed").EqualTo("Angus")
                                              .WithProperty("Weight").EqualTo("1200"));

            Cow angus = manager.GetInstance<Cow>("Angus");

            Assert.IsNotNull(angus);
            Assert.AreEqual("Bessie", angus.Name, "Name");
            Assert.AreEqual(BreedEnum.Angus, angus.Breed, "Breed");
            Assert.AreEqual(1200, angus.Weight, "Weight");
        }
    }
}