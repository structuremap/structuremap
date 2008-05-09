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
    public class PluginGraphBuilder
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

        private readonly ConfigurationParser[] _parsers;
        private readonly Registry[] _registries = new Registry[0];
        private readonly PluginGraph _graph;

        #region constructors

        public PluginGraphBuilder(ConfigurationParser parser)
            : this(new ConfigurationParser[] {parser}, new Registry[0])
        {
        }

        public PluginGraphBuilder(ConfigurationParser[] parsers, Registry[] registries)
        {
            _parsers = parsers;
            _registries = registries;
            _graph = new PluginGraph();
        }

        #endregion

        /// <summary>
        /// Reads the configuration information and returns the PluginGraph definition of
        /// plugin families and plugin's
        /// </summary>
        /// <returns></returns>
        public PluginGraph Build()
        {
            NormalGraphBuilder graphBuilder = new NormalGraphBuilder(_registries, _graph);
            buildPluginGraph(graphBuilder);

            _graph.Seal();

            return _graph;
        }

        private void forAllParsers(Action<ConfigurationParser> action)
        {
            foreach (ConfigurationParser parser in _parsers)
            {
                action(parser);
            }
        }

        private void buildPluginGraph(IGraphBuilder graphBuilder)
        {
            forAllParsers(delegate(ConfigurationParser p) { p.ParseAssemblies(graphBuilder); });

            graphBuilder.PrepareSystemObjects();

            forAllParsers(delegate(ConfigurationParser p)
                              {
                                  p.ParseFamilies(graphBuilder);
                                  p.ParseProfilesAndMachines(graphBuilder);
                                  p.ParseInstances(graphBuilder);
                              });
        }
    }
}