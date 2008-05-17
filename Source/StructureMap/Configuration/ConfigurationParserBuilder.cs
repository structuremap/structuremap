using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using StructureMap.Diagnostics;

namespace StructureMap.Configuration
{
    public class ConfigurationParserBuilder
    {
        private readonly List<XmlNode> _nodes = new List<XmlNode>();
        private readonly List<string> _otherFiles = new List<string>();
        private bool _ignoreDefaultFile = false;
        private readonly GraphLog _log;
        private bool _useAndEnforceExistenceOfDefaultFile = false;
        private bool _pullConfigurationFromAppConfig;


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
            set
            {
                _pullConfigurationFromAppConfig = value;
            }
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
                                     IncludeNode(appConfigNode);
                                 }
                             }).AndLogAnyErrors();

            }

            // TODO -- some error handling here, or somewhere else.  Need to create ConfigurationParser 
            // as soon as the node is added to try to determine errors
            foreach (XmlNode node in _nodes)
            {
                ConfigurationParser parser = new ConfigurationParser(node);
                list.Add(parser);
            }

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

        public void IncludeNode(XmlNode node)
        {
            _nodes.Add(node);
        }

        public static ConfigurationParser[] GetParsers(XmlNode node, GraphLog log)
        {
            ConfigurationParserBuilder builder = new ConfigurationParserBuilder(log);
            builder.IncludeNode(node);
            builder.IgnoreDefaultFile = true;

            return builder.GetParsers();
        }
    }
}