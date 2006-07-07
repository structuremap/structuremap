using System.Reflection;
using System.Xml;
using NUnit.Framework;
using StructureMap.DeploymentTasks;

namespace StructureMap.Testing.DeploymentTasks
{
	[TestFixture]
	public class AddAssemblyTaskTester
	{
		[Test]
		public void WriteAssembly()
		{
			string assemblyName = Assembly.GetExecutingAssembly().FullName;
			string filename = "WriteAssembly.xml";

			XmlDocument document = new XmlDocument();
			document.LoadXml("<StructureMap></StructureMap>");
			document.Save(filename);

			AddAssembly task = new AddAssembly();
			task.ConfigPath = filename;
			task.AssemblyName = assemblyName;

			XmlDocument afterDocument = task.AddAssemblyNode();

			XmlElement assemblyElement = (XmlElement) afterDocument.DocumentElement.FirstChild;
		
			Assert.AreEqual("Assembly", assemblyElement.Name);
			Assert.AreEqual(assemblyName, assemblyElement.GetAttribute("Name"));
		}
	}
}
