using NUnit.Framework;
using Shouldly;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class RootInstanceGraphTester
    {
        [Test]
        public void default_lifecycle_by_default_is_null()
        {
            var root = new RootInstanceGraph(new PluginGraph());
            root.DefaultLifecycleFor(typeof(IGateway)).ShouldBeNull();
        }

        [Test]
        public void default_lifecycle_is_null_if_family_has_no_lifecycle()
        {
            var graph = new PluginGraph();
            graph.Families[typeof (IGateway)].SetDefault(new SmartInstance<StubbedGateway>());

            var root = new RootInstanceGraph(graph);
            root.DefaultLifecycleFor(typeof(IGateway)).ShouldBeNull();
        }

        [Test]
        public void default_lifecycle_is_explicitly_set()
        {
            var graph = new PluginGraph();
            graph.Families[typeof(IGateway)].SetLifecycleTo<SingletonLifecycle>();

            var root = new RootInstanceGraph(graph);
            root.DefaultLifecycleFor(typeof(IGateway)).ShouldBeOfType<SingletonLifecycle>();
        }



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
            pipeline.ToNestedGraph().Transients.ShouldBeOfType<ContainerSpecificObjectCache>();
        }
    }
}