using System.IO;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace StructureMap.DeploymentTasks.Versioning
{
	[TaskName("structuremap.versionreport")]
	public class GenerateVersionReport : Task
	{
		private string _directory;
		private string _outputPath;

		public GenerateVersionReport() : base()
		{

		}

		[TaskAttribute("directory", Required=true)]
		public string Directory
		{
			get { return _directory; }
			set { _directory = value; }
		}

		[TaskAttribute("manifest", Required=true)]
		public string OutputPath
		{
			get { return _outputPath; }
			set { _outputPath = value; }
		}

		protected override void ExecuteTask()
		{
			Log(Level.Info, string.Format("Generating a versioning manifest file for {0} to {1}", _directory, _outputPath));

			DirectoryInfo directoryInfo = new DirectoryInfo(_directory);
			DeployedDirectory deployedDirectory = new DeployedDirectory(directoryInfo);

			deployedDirectory.WriteToXml(_outputPath);
		}
	}
}
