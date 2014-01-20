using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class RootInstanceGraphTester
    {
        [Test]
        public void singleton_is_just_the_plugin_graph_singletons()
        {
            var plugins = new PluginGraph();
            plugins.SingletonCache.ShouldNotBeNull();

            var pipeline = PipelineGraph.BuildRoot(plugins);
            
            pipeline.Singletons.ShouldBeTheSameAs(plugins.SingletonCache);
        }

        [Test]
        public void transient_cache_by_default_is_a_nullo()
        {
            var plugins = new PluginGraph();

            var pipeline = PipelineGraph.BuildRoot(plugins);
            pipeline.Transients.ShouldBeOfType<NulloTransientCache>();
        }

        [Test]
        public void transient_cache_of_nested_pipeline_graph_is_a_stateful_cache()
        {
            var plugins = new PluginGraph();

            var pipeline = PipelineGraph.BuildRoot(plugins);
            pipeline.ToNestedGraph().Transients.ShouldBeOfType<LifecycleObjectCache>();
        }
    }
}