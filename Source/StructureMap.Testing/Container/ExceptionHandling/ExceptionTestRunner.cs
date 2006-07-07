using System;
using System.Xml;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing.Container.ExceptionHandling
{
	public class ExceptionTestRunner
	{
		private XmlDocument doc;
		private InstanceManager _manager;
		private XmlNode _configNode;

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
				Assert.AreEqual(expected, ex.Message);
				return;
			}
			catch (Exception ex)
			{
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
				Assert.AreEqual(expected, ex.Message);
				return;
			}
			catch (Exception ex)
			{
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
			PluginGraphBuilder builder = new PluginGraphBuilder(_configNode);
			PluginGraph graph = builder.Build();
			_manager = new InstanceManager(graph);
		}


	}

}