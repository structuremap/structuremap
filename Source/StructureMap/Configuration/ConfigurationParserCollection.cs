using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace StructureMap.Configuration
{
    // TODO:  3.5 cleanup here
    public delegate XmlNode FetchNodeDelegate();

    public class ConfigurationParserCollection
    {
        private readonly List<FetchNodeDelegate> _fetchers = new List<FetchNodeDelegate>();
        private bool _ignoreDefaultFile = false;
        private readonly List<string> _otherFiles = new List<string>();
        private bool _useAndEnforceExistenceOfDefaultFile = false;

        public bool UseAndEnforceExistenceOfDefaultFile
        {
            get { return _useAndEnforceExistenceOfDefaultFile; }
            set { _useAndEnforceExistenceOfDefaultFile = value; }
        }


        public bool IgnoreDefaultFile
        {
            get { return _ignoreDefaultFile; }
            set { _ignoreDefaultFile = value; }
        }

        public ConfigurationParser[] GetParsers()
        {
            List<ConfigurationParser> list = new List<ConfigurationParser>();

            // Pick up the configuration in the default StructureMap.config
            string pathToStructureMapConfig = StructureMapConfiguration.GetStructureMapConfigurationPath();
            if (shouldUseStructureMapConfigFile(pathToStructureMapConfig))
            {
                addParsersFromFile(pathToStructureMapConfig, list);
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

        private bool shouldUseStructureMapConfigFile(string pathToStructureMapConfig)
        {
            return (_useAndEnforceExistenceOfDefaultFile || File.Exists(pathToStructureMapConfig)) && !_ignoreDefaultFile;
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