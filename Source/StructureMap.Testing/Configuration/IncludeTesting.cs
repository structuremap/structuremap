using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget4;

namespace StructureMap.Testing.Configuration
{
    [TestFixture]
    public class IncludeTesting
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            DataMother.WriteDocument("Master.xml");
            DataMother.WriteDocument("Include1.xml");
            DataMother.WriteDocument("Include2.xml");
        }

        private PluginGraph buildGraph()
        {
            return DataMother.GetDiagnosticPluginGraph("Master.xml");
        }

        [Test]
        public void AddAnInstanceFromMasterConfigToAFamilyInInclude()
        {
            PluginGraph graph = buildGraph();
            PluginFamily family = graph.FindFamily(typeof (IStrategy));
            Assert.IsNotNull(family.GetMemento("Blue"));
            Assert.IsNotNull(family.GetMemento("Red"));
            Assert.IsNotNull(family.GetMemento("DeepTest")); // from include
        }

        [Test]
        public void GetAssembliesFromIncludes()
        {
            PluginGraph graph = buildGraph();

            Assert.AreEqual(4, graph.Assemblies.Count);

            Assert.IsTrue(graph.Assemblies.Contains("StructureMap.Testing.Widget"));
            Assert.IsTrue(graph.Assemblies.Contains("StructureMap.Testing.Widget2"));
            Assert.IsTrue(graph.Assemblies.Contains("StructureMap.Testing.Widget3"));
            Assert.IsTrue(graph.Assemblies.Contains("StructureMap.Testing.Widget4"));
        }

        [Test]
        public void GetFamilyFromIncludeAndMaster()
        {
            PluginGraph graph = buildGraph();


            Assert.IsTrue(graph.ContainsFamily(typeof (IStrategy)));
        }
    }
}