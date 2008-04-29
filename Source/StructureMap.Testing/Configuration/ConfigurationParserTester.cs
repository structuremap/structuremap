using System;
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
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            ConfigurationParser parser = new ConfigurationParser(doc.DocumentElement);

            NormalGraphBuilder builder = new NormalGraphBuilder(new Registry[0]);
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
            ConfigurationParser parser = new ConfigurationParser(document.DocumentElement);

            PluginGraphBuilder builder = new PluginGraphBuilder(parser);
            PluginGraph graph = builder.Build();

            InstanceManager manager = new InstanceManager(graph);

            GrandChild tommy = (GrandChild) manager.CreateInstance(typeof (GrandChild), "Tommy");
            Assert.AreEqual(false, tommy.RightHanded);
            Assert.AreEqual(1972, tommy.BirthYear);

            ColorWidget blue = (ColorWidget) manager.CreateInstance(typeof (IWidget), "Blue");
            Assert.AreEqual("Blue", blue.Color);
        }
    }
}