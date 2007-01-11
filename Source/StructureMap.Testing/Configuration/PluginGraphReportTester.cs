using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;
using StructureMap.Graph;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Configuration
{
	[TestFixture]
	public class PluginGraphReportTester
	{
		[Test]
		public void ImportImplicitChildren()
		{
			PluginGraph pluginGraph = new PluginGraph();
			pluginGraph.Assemblies.Add("StructureMap.Testing.Widget");
			pluginGraph.Assemblies.Add("StructureMap.Testing.Widget2");
			pluginGraph.Assemblies.Add("StructureMap.Testing.Widget3");
			pluginGraph.Assemblies.Add("StructureMap.Testing.Widget4");
			pluginGraph.Assemblies.Add("StructureMap.Testing.Widget5");

			pluginGraph.Seal();

			PluginGraphReport report = new PluginGraphReport();
			report.ImportImplicitChildren(pluginGraph);

			Assert.AreEqual(pluginGraph.PluginFamilies.Count, report.Families.Length);

			foreach (PluginFamily family in pluginGraph.PluginFamilies)
			{
				FamilyToken token = report.FindFamily(family.PluginTypeName);
				Assert.AreEqual(family.Plugins.Count, token.Plugins.Length);
			}
		}

		[Test]
		public void CanSerialize()
		{
			PluginGraphReport report = ObjectMother.Report();

			MemoryStream stream = new MemoryStream();

			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, report);

			stream.Position = 0;
			PluginGraphReport report2 = (PluginGraphReport) formatter.Deserialize(stream);
		
			Assert.IsNotNull(report2);
			Assert.IsFalse(object.ReferenceEquals(report, report2));
		}

		[Test]
		public void FindTemplate()
		{
			PluginGraphReport report = ObjectMother.Report();
			TypePath pluginType = new TypePath(typeof(IWidget));

			TemplateToken theTemplate = new TemplateToken("Template1", "Concrete1", new string[]{"prop1"});
			report.FindFamily(pluginType).AddTemplate(theTemplate);

			TemplateToken actual = report.FindTemplate(pluginType, theTemplate.TemplateKey);

			Assert.AreEqual(theTemplate, actual);
		}

	}
}
