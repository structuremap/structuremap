using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Graph;

namespace StructureMap.Testing
{
    [TestFixture]
    public class ReadingRegistriesFromXmlTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        
        [Test]
        public void handles_failures_gracefully_if_the_registry_cannot_be_loaded()
        {
            //290
            var graph = new PluginGraph();
            var builder = new GraphBuilder(graph);
            builder.AddRegistry("an invalid type name");

            graph.Log.ErrorCount.ShouldEqual(1);
            graph.Log.AssertHasError(290);
        }

        
    }
}