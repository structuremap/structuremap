using NUnit.Framework;
using Shouldly;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Testing
{
    [TestFixture]
    public class PipelineGraph_construction_specs
    {
        [Test]
        public void build_root_for_default_tracking_style()
        {
            var pluginGraph = PluginGraph.CreateRoot();
            pluginGraph.TransientTracking = TransientTracking.DefaultNotTrackedAtRoot;
            var graph = PipelineGraph.BuildRoot(pluginGraph);

            graph.Transients.ShouldBeOfType<NulloTransientCache>();
        }

        [Test]
        public void build_root_for_tracked_transients()
        {
            var pluginGraph = PluginGraph.CreateRoot();
            pluginGraph.TransientTracking = TransientTracking.ExplicitReleaseMode;
            var graph = PipelineGraph.BuildRoot(pluginGraph);

            graph.Transients.ShouldBeOfType<TrackingTransientCache>();
        }


    }
}