using System;
using System.Xml;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Graph.Configuration;

namespace StructureMap
{
    /// <summary>
    /// Reads configuration XML documents and builds the structures necessary to initialize
    /// the InstanceManager/IInstanceFactory/InstanceBuilder/ObjectInstanceActivator objects
    /// </summary>
    public class PluginGraphBuilder : IPluginGraphSource
    {
        private PluginGraph _graph;
        private InstanceDefaultManager _defaultManager;
        private PluginGraphReport _report;
        private ConfigurationParser[] _parsers;

        #region constructors

        /// <summary>
        /// Creates a PluginGraphBuilder from a %lt;StructureMap%gt; node
        /// </summary>
        /// <param name="structureMapNode"></param>
        public PluginGraphBuilder(XmlNode structureMapNode)
        {
            _parsers = new ConfigurationParser[] {new ConfigurationParser(structureMapNode)};
        }


        public PluginGraphBuilder(XmlDocument document) : this(document.DocumentElement)
        {
        }

        /// <summary>
        /// Creates a PluginGraphBuilder that reads configuration from the filePath
        /// </summary>
        /// <param name="filePath">The path to the configuration file</param>
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
        public PluginGraphBuilder() : this(StructureMapConfiguration.GetStructureMapConfigurationPath())
        {
        }

        #endregion

        /// <summary>
        /// Reads the configuration information and returns the PluginGraph definition of
        /// plugin families and plugin's
        /// </summary>
        /// <returns></returns>
        public PluginGraph Build()
        {
            _defaultManager = new InstanceDefaultManager();
            NormalGraphBuilder graphBuilder = new NormalGraphBuilder(_defaultManager);
            PluginGraph pluginGraph = buildPluginGraph(graphBuilder);
            return pluginGraph;
        }

        private PluginGraph buildPluginGraph(IGraphBuilder graphBuilder)
        {
            foreach (ConfigurationParser parser in _parsers)
            {
                parser.ParseAssemblies(graphBuilder);
            }

            graphBuilder.StartFamilies();

            foreach (ConfigurationParser parser in _parsers)
            {
                parser.ParseFamilies(graphBuilder);
            }

            foreach (ConfigurationParser parser in _parsers)
            {
                parser.ParseProfilesAndMachines(graphBuilder);
            }

            graphBuilder.FinishFamilies();

            _defaultManager.ReadDefaultsFromPluginGraph(graphBuilder.PluginGraph);

            foreach (ConfigurationParser parser in _parsers)
            {
                parser.ParseInstances(graphBuilder);
            }

            _graph = graphBuilder.CreatePluginGraph();

            return _graph;
        }


        /// <summary>
        /// Build a PluginGraph with all instances calculated.  Used in the UI and diagnostic tools.
        /// </summary>
        /// <returns></returns>
        public PluginGraph BuildDiagnosticPluginGraph()
        {
            _defaultManager = new InstanceDefaultManager();
            DiagnosticGraphBuilder graphBuilder = new DiagnosticGraphBuilder(_defaultManager);
            buildPluginGraph(graphBuilder);

            _report = graphBuilder.Report;

            return _graph;
        }


        public InstanceDefaultManager DefaultManager
        {
            get
            {
                if (_graph == null)
                {
                    Build();
                }
                return _defaultManager;
            }
        }


        public PluginGraphReport Report
        {
            get
            {
                if (_report == null)
                {
                    BuildDiagnosticPluginGraph();
                }
                return _report;
            }
        }
    }
}