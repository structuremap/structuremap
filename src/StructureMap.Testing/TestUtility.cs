using System;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Testing
{
    public static class TestUtility
    {
        public static void AssertErrorIsLogged(int errorCode, Action<PluginGraph> action)
        {
            var graph = new PluginGraph();
            action(graph);
            graph.Log.AssertHasError(errorCode);
        }
    }
}