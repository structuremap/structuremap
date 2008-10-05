using System.Collections.Generic;
using System.IO;
using System.Xml;
using StructureMap.Diagnostics;

namespace StructureMap.Configuration
{
    public interface IConfigurationParserBuilder
    {
        bool UseAndEnforceExistenceOfDefaultFile { get; set; }
        bool IgnoreDefaultFile { get; set; }
        bool PullConfigurationFromAppConfig { get; set; }
        void IncludeFile(string filename);
        void IncludeNode(XmlNode node, string description);
    }

    public class ConfigurationParserBuilder : IConfigurationParserBuilder
    {
        private readonly GraphLog _log;
        private readonly List<string> _otherFiles = new List<string>();
        private readonly List<ConfigurationParser> _parsers = new List<ConfigurationParser>();
        private bool _ignoreDefaultFile;
        private bool _pullConfigurationFromAppConfig;
        private bool _useAndEnforceExistenceOfDefaultFile;


        public ConfigurationParserBuilder(GraphLog log)
        {
            _log = log;
        }

        #region IConfigurationParserBuilder Members

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

        public void IncludeFile(string filename)
        {
            _otherFiles.Add(filename);
        }


        public void IncludeNode(XmlNode node, string description)
        {
            var parser = new ConfigurationParser(node);
            parser.Description = description;

            _parsers.Add(parser);
        }

        #endregion

        public ConfigurationParser[] GetParsers()
        {
            var list = new List<ConfigurationParser>();

            addConfigurationFromStructureMapConfig(list);
            addConfigurationFromExplicitlyAddedFiles(list);
            addConfigurationFromApplicationConfigFile();

            list.AddRange(_parsers);

            addConfigurationFromIncludeNodes(list);

            return list.ToArray();
        }

        private void addConfigurationFromIncludeNodes(List<ConfigurationParser> list)
        {
            foreach (ConfigurationParser parser in list.ToArray())
            {
                parser.ForEachFile(_log,
                                   filename => _log.Try(() =>
                                   {
                                       ConfigurationParser childParser = ConfigurationParser.FromFile(filename);
                                       list.Add(childParser);
                                   }).AndReportErrorAs(150, filename));
            }
        }

        private void addConfigurationFromApplicationConfigFile()
        {
            if (_pullConfigurationFromAppConfig)
            {
                _log.Try(() =>
                {
                    IList<XmlNode> appConfigNodes = StructureMapConfigurationSection.GetStructureMapConfiguration();
                    foreach (XmlNode appConfigNode in appConfigNodes)
                    {
                        IncludeNode(appConfigNode, string.Empty);
                    }
                }).AndLogAnyErrors();
            }
        }

        private void addConfigurationFromExplicitlyAddedFiles(List<ConfigurationParser> list)
        {
            foreach (string filename in _otherFiles)
            {
                _log.Try(() =>
                {
                    ConfigurationParser parser = ConfigurationParser.FromFile(filename);
                    parser.Description = filename;
                    list.Add(parser);
                }).AndReportErrorAs(160, filename);
            }
        }

        private void addConfigurationFromStructureMapConfig(List<ConfigurationParser> list)
        {
// Pick up the configuration in the default StructureMap.config
            string pathToStructureMapConfig = StructureMapConfiguration.GetStructureMapConfigurationPath();
            if (shouldUseStructureMapConfigFileAt(pathToStructureMapConfig))
            {
                _log.Try(() =>
                {
                    ConfigurationParser parser = ConfigurationParser.FromFile(pathToStructureMapConfig);
                    list.Add(parser);
                }).AndReportErrorAs(100, pathToStructureMapConfig);
            }
        }

        private bool shouldUseStructureMapConfigFileAt(string pathToStructureMapConfig)
        {
            return
                (_useAndEnforceExistenceOfDefaultFile ||
                 File.Exists(pathToStructureMapConfig)) && !_ignoreDefaultFile;
        }


        public static ConfigurationParser[] GetParsers(XmlNode node, GraphLog log)
        {
            var builder = new ConfigurationParserBuilder(log);
            builder.IncludeNode(node, string.Empty);
            builder.IgnoreDefaultFile = true;

            return builder.GetParsers();
        }
    }
}