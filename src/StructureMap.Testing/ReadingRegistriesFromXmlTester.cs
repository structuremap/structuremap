using System.Diagnostics;
using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing
{
    [TestFixture]
    public class ReadingRegistriesFromXmlTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        public void TheXmlFileRegistryWasLoadedInto(IContainer container)
        {
            container.GetInstance<ColorRule>().Color.ShouldEqual("Cornflower");
        }


        [Test]
        public void handles_failures_gracefully_if_the_registry_cannot_be_loaded()
        {
            //290
            var graph = new PluginGraph();
            var builder = new GraphBuilder(graph);
            builder.AddRegistry("an invalid type name");

            graph.Log.ErrorCount.ShouldEqual(1);
            graph.Log.AssertHasError(290);
        }

        [Test]
        public void read_registry_from_xml()
        {
            var document = new XmlDocument();
            document.LoadXml("<StructureMap><Registry></Registry></StructureMap>");
            document.DocumentElement.FirstChild.ShouldBeOfType<XmlElement>().SetAttribute("Type",
                                                                                          typeof (XmlFileRegistry).
                                                                                              AssemblyQualifiedName);

            Debug.WriteLine(document.OuterXml);

            var container = new Container(x => { x.AddConfigurationFromNode(document.DocumentElement); });

            TheXmlFileRegistryWasLoadedInto(container);
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