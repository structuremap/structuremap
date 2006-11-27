using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget4;

namespace StructureMap.Testing.Configuration
{
	[TestFixture]
	public class IncludeTesting
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			DataMother.WriteDocument("Master.xml");
			DataMother.WriteDocument("Include1.xml");
			DataMother.WriteDocument("Include2.xml");
		}

		private PluginGraph buildGraph()
		{
			PluginGraphBuilder builder = new PluginGraphBuilder("Master.xml");
			return builder.Build();
		}

		[Test]
		public void GetAssembliesFromIncludes()
		{
			PluginGraph graph = buildGraph();

			Assert.AreEqual(4, graph.Assemblies.Count);

			Assert.IsTrue(graph.Assemblies.Contains("StructureMap.Testing.Widget"));
			Assert.IsTrue(graph.Assemblies.Contains("StructureMap.Testing.Widget2"));
			Assert.IsTrue(graph.Assemblies.Contains("StructureMap.Testing.Widget3"));
			Assert.IsTrue(graph.Assemblies.Contains("StructureMap.Testing.Widget4"));
		}

		[Test]
		public void GetFamilyFromIncludeAndMaster()
		{
			PluginGraph graph = buildGraph();

			Assert.IsTrue(graph.PluginFamilies.Contains(typeof(IStrategy)));
		}

		[Test]
		public void AddAnInstanceFromMasterConfigToAFamilyInInclude()
		{
			PluginGraph graph = buildGraph();
			PluginFamily family = graph.PluginFamilies[typeof(IStrategy)];
			Assert.IsNotNull(family.Source.GetMemento("Blue"));
			Assert.IsNotNull(family.Source.GetMemento("Red"));
			Assert.IsNotNull(family.Source.GetMemento("DeepTest")); // from include

			
		}
	}
}
