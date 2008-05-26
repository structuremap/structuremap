using System.Collections.Generic;
using System.IO;
using System.Xml;
using StructureMap.Diagnostics;

namespace StructureMap.Configuration
{
    public class ConfigurationParserBuilder
    {
        private readonly GraphLog _log;
        private readonly List<ConfigurationParser> _parsers = new List<ConfigurationParser>();
        private readonly List<string> _otherFiles = new List<string>();
        private bool _ignoreDefaultFile = false;
        private bool _pullConfigurationFromAppConfig;
        private bool _useAndEnforceExistenceOfDefaultFile = false;


        public ConfigurationParserBuilder(GraphLog log)
        {
            _log = log;
        }

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

        public bool PullConfigurationFromAppConfig
        {
            get { return _pullConfigurationFromAppConfig; }
            set { _pullConfigurationFromAppConfig = value; }
        }

        // TODO:  Clean up with 3.5
        public ConfigurationParser[] GetParsers()
        {
            List<ConfigurationParser> list = new List<ConfigurationParser>();

            // Pick up the configuration in the default StructureMap.config
            string pathToStructureMapConfig = StructureMapConfiguration.GetStructureMapConfigurationPath();
            if (shouldUseStructureMapConfigFileAt(pathToStructureMapConfig))
            {
                _log.Try(delegate()
                {
                    ConfigurationParser parser = ConfigurationParser.FromFile(pathToStructureMapConfig);
                    list.Add(parser);
                }).AndReportErrorAs(100, pathToStructureMapConfig);
            }

            foreach (string filename in _otherFiles)
            {
                _log.Try(delegate()
                {
                    ConfigurationParser parser = ConfigurationParser.FromFile(filename);
                    parser.Description = filename;
                    list.Add(parser);
                }).AndReportErrorAs(160, filename);
            }

            if (_pullConfigurationFromAppConfig)
            {
                _log.Try(delegate()
                {
                    IList<XmlNode> appConfigNodes = StructureMapConfigurationSection.GetStructureMapConfiguration();
                    foreach (XmlNode appConfigNode in appConfigNodes)
                    {
                        IncludeNode(appConfigNode, string.Empty);
                    }
                }).AndLogAnyErrors();
            }

            list.AddRange(_parsers);

                foreach (ConfigurationParser parser in list.ToArray())
            {
                parser.ForEachFile(_log,
                                   delegate(string filename)
                                   {
                                       _log.Try(delegate()
                                       {
                                           ConfigurationParser childParser = ConfigurationParser.FromFile(filename);
                                           list.Add(childParser);
                                       }).AndReportErrorAs(150, filename);
                                   });
            }


            return list.ToArray();
        }

        private bool shouldUseStructureMapConfigFileAt(string pathToStructureMapConfig)
        {
            return
                (_useAndEnforceExistenceOfDefaultFile ||
                 File.Exists(pathToStructureMapConfig)) && !_ignoreDefaultFile;
        }


        public void IncludeFile(string filename)
        {
            _otherFiles.Add(filename);
        }


        public void IncludeNode(XmlNode node, string description)
        {
            ConfigurationParser parser = new ConfigurationParser(node);
            parser.Description = description;

            _parsers.Add(parser);
        }

        public static ConfigurationParser[] GetParsers(XmlNode node, GraphLog log)
        {
            ConfigurationParserBuilder builder = new ConfigurationParserBuilder(log);
            builder.IncludeNode(node, string.Empty);
            builder.IgnoreDefaultFile = true;

            return builder.GetParsers();
        }
    }
}