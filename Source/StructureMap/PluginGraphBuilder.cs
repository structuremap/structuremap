using System;
using System.Xml;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StructureMap
{
    /// <summary>
    /// Reads configuration XML documents and builds the structures necessary to initialize
    /// the InstanceManager/IInstanceFactory/InstanceBuilder/ObjectInstanceActivator objects
    /// </summary>
    public class PluginGraphBuilder : IPluginGraphSource
    {
        #region statics

        public static PluginGraph BuildFromXml(XmlDocument document)
        {
            ConfigurationParser[] parsers = ConfigurationParser.GetParsers(document, "");
            PluginGraphBuilder builder = new PluginGraphBuilder(parsers, new Registry[0]);
            return builder.Build();
        }

        public static PluginGraph BuildFromXml(XmlNode structureMapNode)
        {
            ConfigurationParser parser = new ConfigurationParser(structureMapNode);

            PluginGraphBuilder builder = new PluginGraphBuilder(parser);

            return builder.Build();
        }

        #endregion

        private readonly Registry[] _registries = new Registry[0];
        private PluginGraph _graph;
        private ConfigurationParser[] _parsers;

        #region constructors

        public PluginGraphBuilder(ConfigurationParser parser)
            : this(new ConfigurationParser[] {parser}, new Registry[0])
        {
        }

        public PluginGraphBuilder(ConfigurationParser[] parsers, Registry[] registries)
        {
            _parsers = parsers;
            _registries = registries;
        }


        /// <summary>
        /// Creates a PluginGraphBuilder that reads configuration from the filePath
        /// </summary>
        /// <param name="filePath">The path to the configuration file</param>
        [Obsolete("Elimating direct usage of PluginGraphBuilder")]
        public PluginGraphBuilder(string filePath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);

                _parsers = ConfigurationParser.GetParsers(doc, filePath);
            }
            catch (Exception ex)
            {
                throw new StructureMapException(100, filePath, ex);
            }
        }

        /// <summary>
        /// Default constructor reads configuration from the StructureMap.config file
        /// in the application folder
        /// </summary>
        [Obsolete("Elimating direct usage of PluginGraphBuilder")]
        public PluginGraphBuilder() : this(StructureMapConfiguration.GetStructureMapConfigurationPath())
        {
        }

        #endregion

        #region IPluginGraphSource Members

        /// <summary>
        /// Reads the configuration information and returns the PluginGraph definition of
        /// plugin families and plugin's
        /// </summary>
        /// <returns></returns>
        public PluginGraph Build()
        {
            NormalGraphBuilder graphBuilder = new NormalGraphBuilder(_registries);
            PluginGraph pluginGraph = buildPluginGraph(graphBuilder);
            return pluginGraph;
        }

        #endregion

        private PluginGraph buildPluginGraph(IGraphBuilder graphBuilder)
        {
            readAssemblies(graphBuilder);

            readFamilies(graphBuilder);

            foreach (ConfigurationParser parser in _parsers)
            {
                parser.ParseInstances(graphBuilder);
            }

            _graph = graphBuilder.CreatePluginGraph();

            return _graph;
        }

        private void readInstanceDefaults(IGraphBuilder graphBuilder)
        {
            foreach (ConfigurationParser parser in _parsers)
            {
                parser.ParseProfilesAndMachines(graphBuilder);
            }
        }

        private void readFamilies(IGraphBuilder graphBuilder)
        {
            graphBuilder.StartFamilies();

            foreach (ConfigurationParser parser in _parsers)
            {
                parser.ParseFamilies(graphBuilder);
            }

            readInstanceDefaults(graphBuilder);

            
        }

        private void readAssemblies(IGraphBuilder graphBuilder)
        {
            foreach (ConfigurationParser parser in _parsers)
            {
                parser.ParseAssemblies(graphBuilder);
            }
        }
    }
}