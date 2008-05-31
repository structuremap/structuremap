using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Testing
{
    public static class TestUtility
    {
        public static void AssertErrorIsLogged(int errorCode, Action<PluginGraph> action)
        {
            PluginGraph graph = new PluginGraph();
            action(graph);
            graph.Log.AssertHasError(errorCode);
        }

        public static void AssertDescriptionIs(Instance instance, string expected)
        {
            IDiagnosticInstance diagnosticInstance = instance;
            Assert.AreEqual(expected, diagnosticInstance.CreateToken().Description);
        }
    }
}