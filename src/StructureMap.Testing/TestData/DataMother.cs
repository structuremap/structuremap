using System.IO;
using System.Reflection;
using System.Xml;

namespace StructureMap.Testing.TestData
{
    public static class DataMother
    {
        
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
                Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(DataMother), fileName);
            document.Load(stream);

            return document;
        }

        public static void WriteDocument(string fileName)
        {
            XmlDocument document = GetXmlDocument(fileName);
            document.Save(fileName);
        }
    }
}