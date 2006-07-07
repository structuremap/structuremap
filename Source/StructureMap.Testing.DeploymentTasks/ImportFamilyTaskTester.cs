using System.Xml;
using NUnit.Framework;
using StructureMap.DeploymentTasks;
using StructureMap.Testing.XmlWriting;

namespace StructureMap.Testing.DeploymentTasks
{
	[TestFixture]
	public class ImportFamilyTaskTester
	{
		private string _xml1 = "<StructureMap></StructureMap>";
		private string _xml2 = "<StructureMap><Assembly Name=\"SomeAssembly\"/><PluginFamily Assembly=\"SomeAssembly\" Type=\"SomeAssembly.SomeType\"/></StructureMap>";

		[Test]
		public void ImportFamilyNode()
		{
			XmlDocument source = new XmlDocument();
			source.LoadXml(_xml2);


			XmlDocument target = new XmlDocument();
			target.LoadXml(_xml1);

			ImportFamilyTask task = new ImportFamilyTask();
			task.PluginType = "SomeAssembly.SomeType";

			task.ImportFamilyNode(source, target);

			ElementChecker checker = new ElementChecker(source.DocumentElement);
			checker.Check(target.DocumentElement);
		}
	}
}
