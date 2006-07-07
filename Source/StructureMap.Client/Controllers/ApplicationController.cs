using System.IO;
using StructureMap.Configuration;

namespace StructureMap.Client.Controllers
{
	public class ApplicationController
	{
		private readonly IApplicationShell _shell;
		private readonly IReportSource _source;
		private IHTMLSourceFactory _factory;
		

		public ApplicationController(IApplicationShell shell, IReportSource source)
		{
			_shell = shell;
			_source = source;
			_factory = new HTMLSourceFactory();
		}

		public ApplicationController(IApplicationShell shell, IReportSource source, IHTMLSourceFactory factory)
		{
			_shell = shell;
			_source = source;
			_factory = factory;
		}

		public void RefreshReport()
		{
			string configPath = _shell.ConfigurationPath;
			string assemblyFolder = _shell.LockFolders ? Path.GetDirectoryName(configPath) : _shell.AssemblyFolder;

			PluginGraphReport report = _source.FetchReport(configPath, assemblyFolder);

			TreeBuilder builder = new TreeBuilder(report);

			_shell.TopNode = builder.BuildTree();
		}

		public void ShowView(string viewName, GraphObject subject)
		{
			IHTMLSource source = _factory.GetSource(viewName);
			string html = source.BuildHTML(subject);
			_shell.DisplayHTML(html);
		}
	}
}
