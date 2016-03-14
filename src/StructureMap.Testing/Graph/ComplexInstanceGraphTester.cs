using Shouldly;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;
using Xunit;

namespace StructureMap.Testing.Graph
{
    public class ComplexInstanceGraphTester
    {
        [Fact]
        public void lifecycle_for_pluginType_explicitly_set()
        {
            var graph = PluginGraph.CreateRoot();
            var profile = graph.Profile("Red");
            profile.Families[typeof(IGateway)].SetLifecycleTo<SingletonLifecycle>();

            var pipeline = PipelineGraph.BuildRoot(graph).Profiles.For("Red");
            pipeline.Instances.DefaultLifecycleFor(typeof(IGateway))
                .ShouldBeOfType<SingletonLifecycle>();
        }

        [Fact]
        public void lifecyle_for_pluginType_not_explicitly_set_falls_back_to_parent()
        {
            var graph = PluginGraph.CreateRoot();
            graph.Families[typeof(IGateway)].SetLifecycleTo<SingletonLifecycle>();

            var pipeline = PipelineGraph.BuildRoot(graph).Profiles.For("Red");
            pipeline.Instances.DefaultLifecycleFor(typeof(IGateway))
                .ShouldBeOfType<SingletonLifecycle>();
        }

        [Fact]
        public void default_lifecycle_not_set_on_any_level()
        {
            var graph = PluginGraph.CreateRoot();

            var pipeline = PipelineGraph.BuildRoot(graph).Profiles.For("Red");
            pipeline.Instances.DefaultLifecycleFor(typeof(IGateway))
                .ShouldBeNull();
        }
    }
}