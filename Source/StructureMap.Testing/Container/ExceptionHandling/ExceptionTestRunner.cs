using System;
using System.Diagnostics;
using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing.Container.ExceptionHandling
{
    public class ExceptionTestRunner
    {
        private XmlNode _configNode;
        private InstanceManager _manager;
        private XmlDocument doc;

        public ExceptionTestRunner()
        {
            doc = DataMother.GetXmlDocument("ExceptionHandlingTests.xml");
        }

        public void ExecuteExceptionTestFromResetDefaults(int ErrorCode)
        {
            string expected = configureAndFetchMessage(ErrorCode);

            try
            {
                buildInstanceManager();
            }
            catch (StructureMapException ex)
            {
                Debug.WriteLine("Actual:\n" + ex.ToString() + "\n\n\n");

                Assert.AreEqual(expected, ex.Message);
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Actual:\n" + ex.ToString() + "\n\n\n");

                Assert.Fail("Wrong Exception -- " + ex.Message);
            }

            Assert.Fail("No exception.");
        }

        public void ExecuteGetInstance(int ErrorCode, string instanceKey, Type pluginType)
        {
            string expected = configureAndFetchMessage(ErrorCode);

            try
            {
                buildInstanceManager();
                object o = _manager.CreateInstance(pluginType, instanceKey);
            }
            catch (StructureMapException ex)
            {
                Debug.WriteLine("Actual:\n\n" + ex.ToString() + "\n\n");

                Assert.AreEqual(expected, ex.Message);
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Actual:\n\n" + ex.ToString() + "\n\n");
                Assert.Fail("Wrong Exception -- " + ex.Message);
            }

            Assert.Fail("No exception.");
        }

        public void ExecuteGetDefaultInstance(int ErrorCode, Type pluginType)
        {
            string expected = configureAndFetchMessage(ErrorCode);

            try
            {
                buildInstanceManager();
                object o = _manager.CreateInstance(pluginType);
            }
            catch (StructureMapException ex)
            {
                Assert.AreEqual(expected, ex.Message);
                return;
            }
            catch (Exception ex)
            {
                Assert.Fail("Wrong Exception -- " + ex.Message);
            }

            Assert.Fail("No exception.");
        }

        private string configureAndFetchMessage(int ErrorCode)
        {
            string xpath = string.Format("Test[@ErrorCode='{0}']", ErrorCode.ToString());
            XmlNode node = doc.DocumentElement.SelectSingleNode(xpath);
            _configNode = node["StructureMap"];

            XmlElement expectedNode = node["Expectation"];
            string expected = expectedNode.InnerText.Trim();
            expected = expected.Replace("\r\n", "\n");
            return expected;
        }

        private void buildInstanceManager()
        {
            ConfigurationParser parser = new ConfigurationParser(_configNode);
            PluginGraphBuilder builder = new PluginGraphBuilder(parser);
            PluginGraph graph = builder.Build();
            _manager = new InstanceManager(graph);
        }
    }
}