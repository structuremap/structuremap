using NUnit.Framework;
using StructureMap.Graph;

namespace StructureMap.Testing
{
    [TestFixture]
    public class StructureMapConfigurationTester
    {
        [SetUp]
        public void SetUp()
        {
            StructureMapConfiguration.ResetAll();
        }

        [Test]
        public void BuildPluginGraph()
        {
            PluginGraph graph = StructureMapConfiguration.GetPluginGraph();
            Assert.IsNotNull(graph);
        }

        [Test]
        public void BuildDiagnosticPluginGraph()
        {
            PluginGraph graph = StructureMapConfiguration.GetDiagnosticPluginGraph();
            Assert.IsNotNull(graph);
        }
    }
}
