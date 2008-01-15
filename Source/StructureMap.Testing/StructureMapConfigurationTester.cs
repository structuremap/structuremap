using System;
using System.Collections.Generic;
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
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            ObjectFactory.ReInitialize();
            StructureMapConfiguration.ResetAll();
        }

        #endregion

        private static XmlNode createNodeFromText(string outerXml)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(outerXml);
            return document.DocumentElement;
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
         ExpectedException(typeof (ApplicationException),
             ExpectedMessage =
             "StructureMap detected configuration or environmental problems.  Check the StructureMap error file")]
        public void OnStartUpFail()
        {
            StructureMapConfiguration.OnStartUp().FailOnException();
            StructureMapConfiguration.AddInstanceOf<ISomething>().UsingConcreteType<Something>();

            StructureMapConfiguration.GetPluginGraph();
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
        public void SettingsFromAllParentConfigFilesShouldBeIncluded()
        {
            StructureMapConfigurationSection configurationSection = new StructureMapConfigurationSection();

            XmlNode fromMachineConfig =
                createNodeFromText(@"<StructureMap><Assembly Name=""SomeAssembly""/></StructureMap>");
            XmlNode fromWebConfig =
                createNodeFromText(@"<StructureMap><Assembly Name=""AnotherAssembly""/></StructureMap>");

            IList<XmlNode> parentNodes = new List<XmlNode>();
            parentNodes.Add(fromMachineConfig);

            IList<XmlNode> effectiveConfig =
                configurationSection.Create(parentNodes, null, fromWebConfig) as IList<XmlNode>;

            Assert.IsNotNull(effectiveConfig, "A list of configuration nodes should have been returned.");
            Assert.AreEqual(2, effectiveConfig.Count, "Both configurations should have been returned.");
            Assert.AreEqual(fromMachineConfig, effectiveConfig[0]);
            Assert.AreEqual(fromWebConfig, effectiveConfig[1]);
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