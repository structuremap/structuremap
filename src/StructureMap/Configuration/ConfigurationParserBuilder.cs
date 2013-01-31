using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using StructureMap.Diagnostics;

namespace StructureMap.Configuration
{
    public interface IConfigurationParserBuilder
    {
        bool PullConfigurationFromAppConfig { get; set; }
        void IncludeFile(string filename);
        void IncludeNode(XmlNode node, string description);
    }

    public class ConfigurationParserBuilder : IConfigurationParserBuilder
    {
        private readonly GraphLog _log;
        private readonly List<string> _otherFiles = new List<string>();
        private readonly List<ConfigurationParser> _parsers = new List<ConfigurationParser>();
        private bool _pullConfigurationFromAppConfig;


        public ConfigurationParserBuilder(GraphLog log)
        {
            _log = log;
        }

        #region IConfigurationParserBuilder Members
        public bool PullConfigurationFromAppConfig { get { return _pullConfigurationFromAppConfig; } set { _pullConfigurationFromAppConfig = value; } }

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
                string absolutePath = locateFileAsAbsolutePath(filename);
                _log.Try(() =>
                {
                    ConfigurationParser parser = ConfigurationParser.FromFile(absolutePath);
                    parser.Description = absolutePath;
                    list.Add(parser);
                }).AndReportErrorAs(160, absolutePath);
            }
        }


        public static ConfigurationParser[] GetParsers(XmlNode node, GraphLog log)
        {
            var builder = new ConfigurationParserBuilder(log);
            builder.IncludeNode(node, String.Empty);

            return builder.GetParsers();
        }

        private static string locateFileAsAbsolutePath(string filename)
        {
            if (Path.IsPathRooted(filename)) return filename;
            string basePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string configPath = Path.Combine(basePath, filename);

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