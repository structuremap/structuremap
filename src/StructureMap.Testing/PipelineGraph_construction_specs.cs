using Shouldly;
using StructureMap.Graph;
using StructureMap.Pipeline;
using Xunit;

namespace StructureMap.Testing
{
    public class PipelineGraph_construction_specs
    {
        [Fact]
        public void build_root_for_default_tracking_style()
        {
            var pluginGraph = PluginGraph.CreateRoot();
            pluginGraph.TransientTracking = TransientTracking.DefaultNotTrackedAtRoot;
            var graph = PipelineGraph.BuildRoot(pluginGraph);

            graph.Transients.ShouldBeOfType<NulloTransientCache>();
        }

        [Fact]
        public void build_root_for_tracked_transients()
        {
            var pluginGraph = PluginGraph.CreateRoot();
            pluginGraph.TransientTracking = TransientTracking.ExplicitReleaseMode;
            var graph = PipelineGraph.BuildRoot(pluginGraph);

            graph.Transients.ShouldBeOfType<TrackingTransientCache>();
        }
    }
}