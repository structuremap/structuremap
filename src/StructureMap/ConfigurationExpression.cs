using System.Xml;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StructureMap
{
    /// <summary>
    ///     Used as the argument in the Container.Configure() method to describe
    ///     configuration directives and specify the sources of configuration for
    ///     a Container
    /// </summary>
    public class ConfigurationExpression : Registry
    {
        private readonly PluginGraphBuilder _builder = new PluginGraphBuilder();

        internal ConfigurationExpression()
        {
            _builder.Add(this);
        }

        /// <summary>
        ///     If called, directs StructureMap to look for configuration in the App.config in any <StructureMap> node.
        /// </summary>
        public void IncludeConfigurationFromConfigFile()
        {
            ConfigurationParser.FromApplicationConfig().Each(x => _builder.Add(x));
        }

        /// <summary>
        ///     Creates and adds a Registry object of type T.
        /// </summary>
        /// <typeparam name="T">The Registry Type</typeparam>
        public void AddRegistry<T>() where T : Registry, new()
        {
            AddRegistry(new T());
        }

        /// <summary>
        ///     Imports all the configuration from a Registry object
        /// </summary>
        /// <param name="registry"></param>
        public void AddRegistry(Registry registry)
        {
            _builder.Add(registry);
        }

        /// <summary>
        ///     Imports configuration from an Xml file.  The fileName
        ///     must point to an Xml file with valid StructureMap
        ///     configuration
        /// </summary>
        /// <param name="fileName"></param>
        public void AddConfigurationFromXmlFile(string fileName)
        {
            _builder.Add(ConfigurationParser.FromFile(fileName));
        }

        /// <summary>
        ///     Imports configuration directly from an XmlNode.  This
        ///     method was intended for scenarios like Xml being embedded
        ///     into an assembly.  The node must be a 'StructureMap' node
        /// </summary>
        /// <param name="node"></param>
        public void AddConfigurationFromNode(XmlNode node)
        {
            _builder.Add(new ConfigurationParser(node));
        }

        internal PluginGraph BuildGraph()
        {
            return _builder.Build();
        }
    }
}