using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.Xml;
using StructureMap.Testing;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Widget;

namespace StructureMap.Xml.Testing
{

    [TestFixture]
    public class AppConfigTest
    {

        [Test, Ignore("Until we rewrite the Xml config")]
        public void PullConfigurationFromTheAppConfig()
        {
            var c = new Container(x =>
            {
                // Tell StructureMap to look for configuration 
                // from the App.config file
                // The default is false
                x.IncludeConfigurationFromConfigFile();
            });

            c.GetInstance<IThing<string, bool>>()
                .IsType<ColorThing<string, bool>>().Color.ShouldEqual("Cornflower");
        }

        [Test]
        public void SettingsFromAllParentConfigFilesShouldBeIncluded()
        {
            var configurationSection = new StructureMapConfigurationSection();

            XmlNode fromMachineConfig =
                createNodeFromText(@"<StructureMap><Assembly Name=""SomeAssembly""/></StructureMap>");
            XmlNode fromWebConfig =
                createNodeFromText(@"<StructureMap><Assembly Name=""AnotherAssembly""/></StructureMap>");

            IList<XmlNode> parentNodes = new List<XmlNode>();
            parentNodes.Add(fromMachineConfig);

            var effectiveConfig =
                configurationSection.Create(parentNodes, null, fromWebConfig) as IList<XmlNode>;

            Assert.IsNotNull(effectiveConfig, "A list of configuration nodes should have been returned.");
            Assert.AreEqual(2, effectiveConfig.Count, "Both configurations should have been returned.");
            Assert.AreEqual(fromMachineConfig, effectiveConfig[0]);
            Assert.AreEqual(fromWebConfig, effectiveConfig[1]);
        }

        [Test]
        public void read_registry_from_xml()
        {
            var document = new XmlDocument();
            document.LoadXml("<StructureMap><Registry></Registry></StructureMap>");
            document.DocumentElement.FirstChild.ShouldBeOfType<XmlElement>().SetAttribute("Type",
                                                                                          typeof(XmlFileRegistry).
                                                                                              AssemblyQualifiedName);

            Debug.WriteLine(document.OuterXml);

            var container = new Container(x => x.AddConfigurationFromNode(document.DocumentElement));

            container.GetInstance<ColorRule>().Color.ShouldEqual("Cornflower");
        }

        private static XmlNode createNodeFromText(string outerXml)
        {
            var document = new XmlDocument();
            document.LoadXml(outerXml);
            return document.DocumentElement;
        }
    }

    public class XmlFileRegistry : Registry
    {
        public XmlFileRegistry()
        {
            ForConcreteType<ColorRule>().Configure.Ctor<string>("color").Is("Cornflower");
        }
    }

}