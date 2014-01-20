using System.Xml;
using StructureMap.Configuration.DSL;

namespace StructureMap.Configuration.Xml
{
    /// <summary>
    /// Helper extension methods to add XML-based Configuration to a StructureMap Container
    /// </summary>
    public static class ConfigurationExtensions
    {

        /// <summary>
        ///     If called, directs StructureMap to look for configuration in the App.config in any &lt;StructureMap&gt; node.
        /// </summary>
        public static void IncludeConfigurationFromConfigFile(this IRegistry registry)
        {
            foreach (var parser in ConfigurationParser.FromApplicationConfig())
              registry.Polices.Configure(parser);
        }

        /// <summary>
        ///     Imports configuration from an Xml file.  The fileName
        ///     must point to an Xml file with valid StructureMap
        ///     configuration
        /// </summary>
        public static void AddConfigurationFromXmlFile(this IRegistry registry, string fileName)
        {
            registry.Polices.Configure(ConfigurationParser.FromFile(fileName));
        }

        /// <summary>
        ///     Imports configuration directly from an XmlNode.  This
        ///     method was intended for scenarios like Xml being embedded
        ///     into an assembly.  The node must be a 'StructureMap' node
        /// </summary>
        public static void AddConfigurationFromNode(this IRegistry registry, XmlNode node)
        {
            registry.Polices.Configure(new ConfigurationParser(node));
        }
    }
}