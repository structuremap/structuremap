using NUnit.Framework;
using Shouldly;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class ComplexInstanceGraphTester
    {
        [Test]
        public void lifecycle_for_pluginType_explicitly_set()
        {
            var graph = new PluginGraph();
            var profile = graph.Profile("Red");
            profile.Families[typeof(IGateway)].SetLifecycleTo<SingletonLifecycle>();

            var pipeline = PipelineGraph.BuildRoot(graph).Profiles.For("Red");
            pipeline.Instances.DefaultLifecycleFor(typeof (IGateway))
                .ShouldBeOfType<SingletonLifecycle>();
        }

        [Test]
        public void lifecyle_for_pluginType_not_explicitly_set_falls_back_to_parent()
        {
            var graph = new PluginGraph();
            graph.Families[typeof(IGateway)].SetLifecycleTo<SingletonLifecycle>();

            var pipeline = PipelineGraph.BuildRoot(graph).Profiles.For("Red");
            pipeline.Instances.DefaultLifecycleFor(typeof(IGateway))
                .ShouldBeOfType<SingletonLifecycle>();
        }

        [Test]
        public void default_lifecycle_not_set_on_any_level()
        {
            var graph = new PluginGraph();

            var pipeline = PipelineGraph.BuildRoot(graph).Profiles.For("Red");
            pipeline.Instances.DefaultLifecycleFor(typeof(IGateway))
                .ShouldBeNull();
        }
    }
}