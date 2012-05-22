using System;
using System.Collections.Generic;
using System.Xml;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Diagnostics;
using StructureMap.Graph;

namespace StructureMap
{
    /// <summary>
    /// Used as the argument in the Container.Configure() method to describe
    /// configuration directives and specify the sources of configuration for
    /// a Container
    /// </summary>
    public class ConfigurationExpression : Registry
    {
        [CLSCompliant(false)]
        protected readonly GraphLog _log = new GraphLog();

        [CLSCompliant(false)]
        protected readonly ConfigurationParserBuilder _parserBuilder;

        private readonly List<Registry> _registries = new List<Registry>();

        internal ConfigurationExpression()
        {
            _parserBuilder = new ConfigurationParserBuilder(_log);
            _parserBuilder.IgnoreDefaultFile = true;
            _parserBuilder.PullConfigurationFromAppConfig = false;

            _registries.Add(this);
        }

        /// <summary>
        /// If true, directs StructureMap to look for configuration in the App.config.
        /// The default value is false.
        /// </summary>
        public bool IncludeConfigurationFromConfigFile { set { _parserBuilder.UseAndEnforceExistenceOfDefaultFile = value; } }

        /// <summary>
        /// Creates and adds a Registry object of type T.  
        /// </summary>
        /// <typeparam name="T">The Registry Type</typeparam>
        public void AddRegistry<T>() where T : Registry, new()
        {
            AddRegistry(new T());
        }

        /// <summary>
        /// Imports all the configuration from a Registry object
        /// </summary>
        /// <param name="registry"></param>
        public void AddRegistry(Registry registry)
        {
            _registries.Add(registry);
        }

        /// <summary>
        /// Imports configuration from an Xml file.  The fileName
        /// must point to an Xml file with valid StructureMap
        /// configuration
        /// </summary>
        /// <param name="fileName"></param>
        public void AddConfigurationFromXmlFile(string fileName)
        {
            _parserBuilder.IncludeFile(fileName);
        }

        /// <summary>
        /// Imports configuration directly from an XmlNode.  This
        /// method was intended for scenarios like Xml being embedded
        /// into an assembly.  The node must be a 'StructureMap' node
        /// </summary>
        /// <param name="node"></param>
        public void AddConfigurationFromNode(XmlNode node)
        {
            _parserBuilder.IncludeNode(node, "Xml configuration");
        }

        internal PluginGraph BuildGraph()
        {
            ConfigurationParser[] parsers = _parserBuilder.GetParsers();
            var builder = new PluginGraphBuilder(parsers, _registries.ToArray(), _log);

            return builder.Build();
        }
    }
}