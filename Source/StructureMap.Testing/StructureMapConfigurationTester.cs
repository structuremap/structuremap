using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Testing.GenericWidgets;

namespace StructureMap.Testing
{
    [TestFixture]
    public class StructureMapConfigurationTester
    {
        [SetUp]
        public void SetUp()
        {
            ObjectFactory.ReInitialize();
            StructureMapConfiguration.ResetAll();
        }

        [Test]
        public void BuildPluginGraph()
        {
            PluginGraph graph = StructureMapConfiguration.GetPluginGraph();
            Assert.IsNotNull(graph);
        }

        [Test]
        public void BuildReport()
        {
            PluginGraphReport report = StructureMapConfiguration.GetDiagnosticReport();
            Assert.IsNotNull(report);
        }

        [Test,
        ExpectedException(typeof(ApplicationException), ExpectedMessage = "StructureMap detected configuration or environmental problems.  Check the StructureMap error file")]
        public void OnStartUpFail()
        {
            StructureMapConfiguration.OnStartUp().FailOnException();
            StructureMapConfiguration.AddInstanceOf<ISomething>().UsingConcreteType<Something>();

            StructureMapConfiguration.GetPluginGraph();
        }

        [Test]
        public void WriteAllFile()
        {
            string filePath = "all.txt";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            StructureMapConfiguration.OnStartUp().WriteAllTo(filePath);
            StructureMapConfiguration.AddInstanceOf<ISomething>().UsingConcreteType<Something>();

            StructureMapConfiguration.GetPluginGraph();

            Assert.IsTrue(File.Exists(filePath));
        }


        [Test]
        public void WriteProblems()
        {
            string filePath = "problems.txt";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            StructureMapConfiguration.OnStartUp().WriteProblemsTo(filePath);
            StructureMapConfiguration.AddInstanceOf<ISomething>().UsingConcreteType<Something>();

            StructureMapConfiguration.GetPluginGraph();

            Assert.IsTrue(File.Exists(filePath));
        }


        [Test]
        public void PullConfigurationFromTheAppConfig()
        {
            StructureMapConfiguration.UseDefaultStructureMapConfigFile = false;
            StructureMapConfiguration.PullConfigurationFromAppConfig = true;

            ColorThing<string, bool> thing =
                (ColorThing<string, bool>) ObjectFactory.GetInstance<IThing<string, bool>>();
            Assert.AreEqual("Cornflower", thing.Color, "Cornflower is the color from the App.config file");
        }

        [Test]
        public void AppConfigShouldIncludeSettingsFromParentConfig()
        {
            StructureMapConfigurationSection configurationSection = new StructureMapConfigurationSection();

            XmlNode fromMachineConfig = createNodeFromText(@"<StructureMap><Assembly Name=""SomeAssembly""/></StructureMap>");
            XmlNode fromWebConfig = createNodeFromText(@"<StructureMap><Assembly Name=""AnotherAssembly""/></StructureMap>");

            XmlNode effectiveConfig = configurationSection.Create(fromMachineConfig, null, fromWebConfig) as XmlNode;

            Assert.IsNotNull(effectiveConfig, "A configuration node should have been returned.");
            Assert.AreEqual(2, effectiveConfig.ChildNodes.Count, "Both Assembly entries should have been returned.");
            Assert.IsTrue(hasAttributeValue("Name", "SomeAssembly", effectiveConfig.ChildNodes[0]), "The parent Assembly entry should have been returned first.");
            Assert.IsTrue(hasAttributeValue("Name", "AnotherAssembly", effectiveConfig.ChildNodes[1]), "The child Assembly entry should have been returned second.");

        }

        private static XmlNode createNodeFromText(string outerXml)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(outerXml);
            return document.DocumentElement;
        }

        private static bool hasAttributeValue(string attributeName, string attributeValue, XmlNode node)
        {
            XmlAttribute namedAttribute = node.Attributes[attributeName];
            if (namedAttribute == null) return false;
            return namedAttribute.Value == attributeValue;
        }
    }

    public interface ISomething
    {
    }

    public class Something : ISomething
    {
        public Something()
        {
            throw new ApplicationException("You can't make me!");
        }
    }


}