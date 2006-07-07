using StructureMap.Configuration;

namespace StructureMap.Client.Controllers
{
	public interface IReportSource
	{
		PluginGraphReport FetchReport(string configurationPath, string assemblyFolder);
	}
}
