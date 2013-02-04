using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using StructureMap.Configuration;
using StructureMap.Graph;

namespace StructureMap.Testing.TestData
{
    public class DataMother
    {
        private static readonly ArrayList _files = new ArrayList();


        private DataMother()
        {
        }

        public static Container BuildContainerForXml(string xml)
        {
            PluginGraph graph = BuildPluginGraphFromXml(xml);
            return new Container(graph);
        }

        public static PluginGraph BuildPluginGraphFromXml(string xml)
        {
            XmlDocument document = BuildDocument(xml);

            var parser = new ConfigurationParser(document.DocumentElement);
            var builder = new PluginGraphBuilder(parser);
            return builder.Build();
        }

        public static XmlDocument BuildDocument(string xml)
        {
            xml = xml.Replace("'", "\"");
            var document = new XmlDocument();
            document.LoadXml(xml);
            return document;
        }


        public static void BackupStructureMapConfig()
        {
            if (File.Exists("StructureMap.config.bak")) File.Delete("StructureMap.config.bak");
            File.Copy("StructureMap.config", "StructureMap.config.bak");
        }

        public static void RestoreStructureMapConfig()
        {
            if (!File.Exists("StructureMap.config"))
            {
                File.Copy("StructureMap.config.bak", "StructureMap.config");
            }
        }

        public static void RemoveStructureMapConfig()
        {
            if (File.Exists("StructureMap.config")) File.Delete("StructureMap.config");
        }

        public static XmlDocument GetXmlDocument(string fileName)
        {
            var document = new XmlDocument();

            Stream stream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream(new DataMother().GetType(), fileName);
            document.Load(stream);

            return document;
        }

        public static PluginGraph GetPluginGraph(string fileName)
        {
            XmlDocument document = GetXmlDocument(fileName);
            var parser = new ConfigurationParser(document.DocumentElement);
            var builder = new PluginGraphBuilder(parser);

            return builder.Build();
        }

        public static void WriteDocument(string fileName, string xml)
        {
            var document = new XmlDocument();
            document.LoadXml(xml);

            document.Save(fileName);
        }

        public static void WriteDocument(string fileName)
        {
            XmlDocument document = GetXmlDocument(fileName);
            document.Save(fileName);

            _files.Add(fileName);
        }

        public static void CleanUp()
        {
            foreach (string fileName in _files)
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}