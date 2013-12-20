using System.IO;
using System.Reflection;
using System.Xml;
using StructureMap.Configuration.Xml;
using StructureMap.Graph;

namespace StructureMap.Xml.Testing.TestData
{

    public class DataMother
    {

        public static Container BuildContainerForXml(string xml)
        {
            PluginGraph graph = BuildPluginGraphFromXml(xml);
            return new Container(graph);
        }

        public static PluginGraph BuildPluginGraphFromXml(string xml)
        {
            XmlDocument document = BuildDocument(xml);

            var builder = new PluginGraphBuilder();
            builder.Add(new ConfigurationParser(document.DocumentElement));
            return builder.Build();
        }

        public static PluginGraph GetPluginGraph(string fileName)
        {
            XmlDocument document = GetXmlDocument(fileName);
            var parser = new ConfigurationParser(document.DocumentElement);
            var builder = new PluginGraphBuilder();
            builder.Add(parser);

            return builder.Build();
        }

        public static XmlDocument BuildDocument(string xml)
        {
            xml = xml.Replace("'", "\"");
            var document = new XmlDocument();
            document.LoadXml(xml);
            return document;
        }

        public static XmlDocument GetXmlDocument(string fileName)
        {
            var document = new XmlDocument();

            Stream stream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(DataMother), fileName);
            document.Load(stream);

            return document;
        }

        public static void WriteDocument(string fileName)
        {
            var document = GetXmlDocument(fileName);
            document.Save(fileName);
        }
    }
}