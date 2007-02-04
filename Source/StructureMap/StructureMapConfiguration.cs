using System;
using System.IO;
using StructureMap.Configuration;
using StructureMap.Graph;

namespace StructureMap
{
    public static class StructureMapConfiguration
    {
        private const string CONFIG_FILE_NAME = "StructureMap.config";
        private static ConfigurationParserCollection _collection = new ConfigurationParserCollection();

        /// <summary>
        /// Returns the path to the StructureMap.config file
        /// </summary>
        /// <returns></returns>
        public static string GetStructureMapConfigurationPath()
        {
            string basePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string configPath = Path.Combine(basePath, CONFIG_FILE_NAME);

            if (!File.Exists(configPath))
            {
                configPath = Path.Combine(basePath, "bin");
                configPath = Path.Combine(configPath, CONFIG_FILE_NAME);

                if (!File.Exists(configPath))
                {
                    configPath = Path.Combine(basePath, @"..\" + CONFIG_FILE_NAME);
                }
            }

            return configPath;
        }


        public static void ResetAll()
        {
            _collection = new ConfigurationParserCollection();
        }

        public static PluginGraph GetPluginGraph()
        {
            PluginGraphBuilder builder = createBuilder();
            return builder.Build();
        }

        private static PluginGraphBuilder createBuilder()
        {
            ConfigurationParser[] parsers = _collection.GetParsers();
            return new PluginGraphBuilder(parsers);
        }

        public static PluginGraph GetDiagnosticPluginGraph()
        {
            PluginGraphBuilder builder = createBuilder();
            return builder.BuildDiagnosticPluginGraph();            
        }
    }
}