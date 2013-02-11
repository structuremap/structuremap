using System;
using System.Xml;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Interceptors;

namespace StructureMap
{
    public interface IInitializationExpression : IRegistry
    {
        
        /// <summary>
        /// Imports configuration from an Xml file.  The fileName
        /// must point to an Xml file with valid StructureMap
        /// configuration
        /// </summary>
        /// <param name="fileName"></param>
        void AddConfigurationFromXmlFile(string fileName);

        /// <summary>
        /// Imports configuration directly from an XmlNode.  This
        /// method was intended for scenarios like Xml being embedded
        /// into an assembly.  The node must be a 'StructureMap' node
        /// </summary>
        /// <param name="node"></param>
        void AddConfigurationFromNode(XmlNode node);

        /// <summary>
        /// If called, directs StructureMap to look for configuration in the App.config in any <StructureMap> node.
        /// </summary>
        void IncludeConfigurationFromConfigFile();

        /// <summary>
        /// Creates and adds a Registry object of type T.  
        /// </summary>
        /// <typeparam name="T">The Registry Type</typeparam>
        void AddRegistry<T>() where T : Registry, new();

        /// <summary>
        /// Imports all the configuration from a Registry object
        /// </summary>
        /// <param name="registry"></param>
        void AddRegistry(Registry registry);

    }

    public class InitializationExpression : ConfigurationExpression, IInitializationExpression
    {
        internal InitializationExpression()
        {
            DefaultProfileName = string.Empty;
        }

        public string DefaultProfileName { get; set; }
    }
}