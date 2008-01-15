using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace StructureMap.Configuration
{
    public delegate XmlNode FetchNodeDelegate();

    public class ConfigurationParserCollection
    {
        private List<FetchNodeDelegate> _fetchers = new List<FetchNodeDelegate>();
        private List<string> _otherFiles = new List<string>();
        private bool _useDefaultFile = true;

        public bool UseDefaultFile
        {
            get { return _useDefaultFile; }
            set { _useDefaultFile = value; }
        }

        public ConfigurationParser[] GetParsers()
        {
            List<ConfigurationParser> list = new List<ConfigurationParser>();

            if (_useDefaultFile)
            {
                addParsersFromFile(StructureMapConfiguration.GetStructureMapConfigurationPath(), list);
            }

            foreach (string file in _otherFiles)
            {
                addParsersFromFile(file, list);
            }

            foreach (FetchNodeDelegate fetcher in _fetchers)
            {
                XmlNode node = fetcher();
                addParsersFromDocument(node, string.Empty, list);
            }

            return list.ToArray();
        }

        private static void addParsersFromFile(string filename, List<ConfigurationParser> list)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);

                string includePath = Path.GetDirectoryName(filename);
                addParsersFromDocument(doc.DocumentElement, includePath, list);
            }
            catch (Exception ex)
            {
                throw new StructureMapException(100, filename, ex);
            }
        }

        private static void addParsersFromDocument(XmlNode node, string includePath, List<ConfigurationParser> list)
        {
            ConfigurationParser[] parsers = ConfigurationParser.GetParsers(node, includePath);
            list.AddRange(parsers);
        }

        public void IncludeFile(string filename)
        {
            _otherFiles.Add(filename);
        }

        public void IncludeNode(FetchNodeDelegate fetcher)
        {
            _fetchers.Add(fetcher);
        }
    }
}