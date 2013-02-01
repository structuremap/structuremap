using System;
using System.IO;
using System.Xml;
using StructureMap.Diagnostics;
using StructureMap.Source;

namespace StructureMap.Configuration
{
    public class ConfigurationParser : XmlConstants
    {
        #region statics

        public static ConfigurationParser FromFile(string filename)
        {
            var document = new XmlDocument();
            document.Load(filename);

            XmlNode structureMapNode = document.SelectSingleNode("//" + STRUCTUREMAP);
            if (structureMapNode == null)
            {
                throw new StructureMapException(155, filename);
            }

            var parser = new ConfigurationParser(structureMapNode);
            parser.FilePath = filename;

            return parser;
        }

        #endregion

        private readonly XmlMementoCreator _mementoCreator;
        private readonly XmlNode _structureMapNode;
        private string _filePath = string.Empty;
        public string Description = string.Empty;

        public ConfigurationParser(XmlNode structureMapNode)
        {
            _structureMapNode = structureMapNode;

            _mementoCreator = new XmlMementoCreator();
        }

        public string Id
        {
            get
            {
                XmlAttribute att = _structureMapNode.Attributes["Id"];
                return att == null ? string.Empty : att.Value;
            }
        }


        public string FilePath { get { return _filePath; } set { _filePath = value; } }

        public void ForEachFile(GraphLog log, Action<string> action)
        {
            string includePath = getIncludePath();

            // Find the text in every child node of _structureMapNode and
            // perform an action with that text
            _structureMapNode.ForTextInChild("Include/@File").Do(fileName =>
            {
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
            _structureMapNode.ForTextInChild("Registry/@Type").Do(name => builder.AddRegistry(name));
        }


        private XmlExtensions.XmlNodeExpression forEachNode(string xpath)
        {
            return _structureMapNode.ForEachChild(xpath);
        }

        public void Parse(IGraphBuilder builder)
        {
            var instanceParser = new InstanceParser(builder, _mementoCreator);

            forEachNode(DEFAULT_INSTANCE).Do(instanceParser.ParseDefaultElement);
            forEachNode(ADD_INSTANCE_NODE).Do(instanceParser.ParseInstanceElement);
        }
    }
}