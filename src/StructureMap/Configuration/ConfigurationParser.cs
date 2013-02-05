using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Source;

namespace StructureMap.Configuration
{
    public class ConfigurationParser : XmlConstants, IPluginGraphConfiguration
    {
        private readonly XmlNode _structureMapNode;
        public string Description = string.Empty;
        private string _filePath = string.Empty;

        public ConfigurationParser(XmlNode structureMapNode)
        {
            _structureMapNode = structureMapNode;
        }

        public string Id
        {
            get
            {
                XmlAttribute att = _structureMapNode.Attributes["Id"];
                return att == null ? string.Empty : att.Value;
            }
        }


        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        // TODO -- set the description
        public static ConfigurationParser FromFile(string filename)
        {
            var document = new XmlDocument();
            document.Load(filename);

            XmlNode structureMapNode = document.SelectSingleNode("//" + STRUCTUREMAP);
            if (structureMapNode == null)
            {
                throw new StructureMapException(155, filename);
            }

            return new ConfigurationParser(structureMapNode) {FilePath = filename};
        }

        public static InstanceMemento CreateMemento(XmlNode node)
        {
            XmlNode clonedNode = node.CloneNode(true);
            return new XmlAttributeInstanceMemento(clonedNode);
        }

        public void ForEachFile(GraphLog log, Action<string> action)
        {
            string includePath = getIncludePath();

            // Find the text in every child node of _structureMapNode and
            // perform an action with that text
            _structureMapNode.ForTextInChild("Include/@File").Do(fileName => {
                string includedFile = Path.Combine(includePath, fileName);
                action(includedFile);
            });
        }

        private string getIncludePath()
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                return string.Empty;
            }

            return Path.GetDirectoryName(_filePath);
        }

        public void ParseRegistries(IGraphBuilder builder)
        {
            _structureMapNode.ForTextInChild("Registry/@Type").Do(builder.AddRegistry);
        }


        private XmlExtensions.XmlNodeExpression forEachNode(string xpath)
        {
            return _structureMapNode.ForEachChild(xpath);
        }

        public void Parse(IGraphBuilder builder)
        {
            var instanceParser = new InstanceParser(builder);

            forEachNode("Alias").Do(instanceParser.ParseAlias);
            forEachNode(DEFAULT_INSTANCE).Do(instanceParser.ParseDefaultElement);
            forEachNode(ADD_INSTANCE_NODE).Do(instanceParser.ParseInstanceElement);
        }

        void IPluginGraphConfiguration.Configure(PluginGraph graph)
        {
            var builder = new GraphBuilder(graph);

            ParseRegistries(builder);
            Parse(builder);
        }

        // TODO -- set the description
        public static IEnumerable<ConfigurationParser> FromApplicationConfig()
        {
            IList<XmlNode> appConfigNodes = StructureMapConfigurationSection.GetStructureMapConfiguration();
            foreach (XmlNode appConfigNode in appConfigNodes)
            {
                yield return new ConfigurationParser(appConfigNode);

            }
        }
    }
}