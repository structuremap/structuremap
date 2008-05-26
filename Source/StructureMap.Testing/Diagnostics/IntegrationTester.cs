using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Graph;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing.Diagnostics
{
    [TestFixture]
    public class IntegrationTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Smoke_test_error_report_on_InstanceManager()
        {
            PluginGraph graph = DataMother.GetDiagnosticPluginGraph("Invalid.config");

        }
    }
}
