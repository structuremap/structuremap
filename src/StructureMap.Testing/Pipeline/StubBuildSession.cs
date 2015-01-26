using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Pipeline
{
    public class StubBuildSession : BuildSession
    {
        private static readonly PluginGraph _pluginGraph = new PluginGraph();
        private static readonly IPipelineGraph _pipeline;

        static StubBuildSession()
        {
            _pipeline = PipelineGraph.BuildRoot(_pluginGraph);
        }

        public StubBuildSession()
            : base(_pipeline)
        {
        }

        public object CreateInstance(string typeName, IConfiguredInstance instance)
        {
            throw new NotImplementedException();
        }

        public object CreateInstance(string typeName)
        {
            throw new NotImplementedException();
        }

        public object CreateInstance(Type pluginType, IConfiguredInstance instance)
        {
            throw new NotImplementedException();
        }
    }
}