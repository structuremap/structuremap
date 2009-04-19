using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture] public class ObjectBuilderTester
    {
        private PluginGraph graph;
        private PipelineGraph pipeline;
        private InterceptorLibrary library;
        private IObjectCache theDefaultCache;
        private ObjectBuilder builder;

        [SetUp] public void SetUp()
        {
            graph = new PluginGraph();
            pipeline = new PipelineGraph(graph);
            library = new InterceptorLibrary();

            theDefaultCache = MockRepository.GenerateMock<IObjectCache>();

            builder = new ObjectBuilder(pipeline, library, theDefaultCache);
        }

        [Test] public void FIRSTTEST()
        {
            
        }
    }
}