using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Source;
using StructureMap.Testing.Widget2;

namespace StructureMap.Testing.Container
{
    [TestFixture]
    public class EnumerationTester
    {
        public EnumerationTester()
        {
        }


        [Test]
        public void BuildClassWithEnumeration()
        {
            PluginGraph graph = new PluginGraph();


            PluginFamily family = graph.FindFamily(typeof (Cow));
            family.AddPlugin(typeof (Cow), "Default");

            InstanceManager manager = new InstanceManager(graph);

            ConfiguredInstance instance = new ConfiguredInstance()
                .WithConcreteKey("Default").WithName("Angus")
                .WithProperty("Name").EqualTo("Bessie")
                .WithProperty("Breed").EqualTo("Angus")
                .WithProperty("Weight").EqualTo("1200");

            
            
            manager.AddInstance<Cow>(instance);

            Cow angus = manager.CreateInstance<Cow>("Angus");

            Assert.IsNotNull(angus);
            Assert.AreEqual("Bessie", angus.Name, "Name");
            Assert.AreEqual(BreedEnum.Angus, angus.Breed, "Breed");
            Assert.AreEqual(1200, angus.Weight, "Weight");
        }
    }
}