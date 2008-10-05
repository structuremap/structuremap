using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Configuration
{
    [TestFixture]
    public class ConfigurationParserTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var parser = new ConfigurationParser(doc.DocumentElement);

            var builder = new GraphBuilder(new Registry[0]);
            parser.ParseProfilesAndMachines(builder);
        }

        #endregion

        private string xml =
            @"
<StructureMap DefaultProfile=""Blue"">
	<Profile Name=""Blue"">
		<Override Type=""StructureMap.Testing.Widget.IWidget,StructureMap.Testing.Widget"" DefaultKey=""Blue""/>
		<Override Type=""StructureMap.Testing.Widget.Rule,StructureMap.Testing.Widget"" DefaultKey=""Blue""/>
		<Override Type=""StructureMap.Testing.Widget4.IStrategy,StructureMap.Testing.Widget4"" DefaultKey=""Blue""/>
	</Profile>

	<Profile Name=""Green"">
		<Override Type=""StructureMap.Testing.Widget.IWidget,StructureMap.Testing.Widget"" DefaultKey=""Green""/>
		<Override Type=""StructureMap.Testing.Widget.Rule,StructureMap.Testing.Widget"" DefaultKey=""Green""/>
	</Profile>	

	<Machine Name=""GREEN-BOX"" Profile=""Green""/>
	
	<Machine Name=""SERVER"">
		<Override Type=""StructureMap.Testing.Widget.IWidget,StructureMap.Testing.Widget"" DefaultKey=""Orange""/>
		<Override Type=""StructureMap.Testing.Widget4.IStrategy,StructureMap.Testing.Widget"" DefaultKey=""Blue""/>
	</Machine>
</StructureMap>";

        [Test]
        public void SwitchToAttributeNormalizedMode()
        {
            XmlDocument document = DataMother.GetXmlDocument("AttributeNormalized.xml");
            var parser = new ConfigurationParser(document.DocumentElement);

            var builder = new PluginGraphBuilder(parser);
            PluginGraph graph = builder.Build();

            var manager = new Container(graph);

            var tommy = (GrandChild) manager.GetInstance(typeof (GrandChild), "Tommy");
            Assert.AreEqual(false, tommy.RightHanded);
            Assert.AreEqual(1972, tommy.BirthYear);

            var blue = (ColorWidget) manager.GetInstance(typeof (IWidget), "Blue");
            Assert.AreEqual("Blue", blue.Color);
        }
    }
}