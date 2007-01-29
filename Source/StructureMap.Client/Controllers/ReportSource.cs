using StructureMap.Configuration;
using StructureMap.Graph;

namespace StructureMap.Client.Controllers
{
    public class ReportSource : IReportSource
    {
        public PluginGraphReport FetchReport(string configurationPath, string assemblyFolder)
        {
            RemoteGraphContainer container = new RemoteGraphContainer(configurationPath, assemblyFolder);
            RemoteGraph graph = container.GetRemoteGraph();

            return graph.GetReport();
        }
    }
}