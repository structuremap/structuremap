using System;
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
                parser.ForEachFile(_log, filename => _log.Try(() =>
                {
                    ConfigurationParser childParser = ConfigurationParser.FromFile(filename);
                    list.Add(childParser);
                })
                .AndReportErrorAs(150, filename));
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
                        IncludeNode(appConfigNode, String.Empty);
                    }
                }).AndLogAnyErrors();
            }
        }

        private void addConfigurationFromExplicitlyAddedFiles(ICollection<ConfigurationParser> list)
        {
            foreach (string filename in _otherFiles)
            {
                var absolutePath = locateFileAsAbsolutePath(filename);
                _log.Try(() =>
                {
                    ConfigurationParser parser = ConfigurationParser.FromFile(absolutePath);
                    parser.Description = absolutePath;
                    list.Add(parser);
                }).AndReportErrorAs(160, absolutePath);
            }
        }

        private void addConfigurationFromStructureMapConfig(ICollection<ConfigurationParser> list)
        {
// Pick up the configuration in the default StructureMap.config
            string pathToStructureMapConfig = GetStructureMapConfigurationPath();
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
            builder.IncludeNode(node, String.Empty);
            builder.IgnoreDefaultFile = true;

            return builder.GetParsers();
        }

        /// <summary>
        /// The name of the default configuration file. The value is always <c>StructurMap.config</c>
        /// </summary>
        public static readonly string DefaultConfigurationFilename = "StructureMap.config";

        /// <summary>
        /// Returns the absolute path to the StructureMap.config file
        /// </summary>
        /// <returns></returns>
        public static string GetStructureMapConfigurationPath()
        {
            return locateFileAsAbsolutePath(DefaultConfigurationFilename);
        }

        private static string locateFileAsAbsolutePath(string filename)
        {
            if (Path.IsPathRooted(filename)) return filename;
            var basePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var configPath = Path.Combine(basePath, filename);

            if (!File.Exists(configPath))
            {
                configPath = Path.Combine(basePath, "bin");
                configPath = Path.Combine(configPath, filename);

                if (!File.Exists(configPath))
                {
                    configPath = Path.Combine(basePath, "..");
                    configPath = Path.Combine(configPath, filename);
                }
            }

            return configPath;
        }
    }
}