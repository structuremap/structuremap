using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Source;

namespace StructureMap.Configuration
{
    public class ConfigurationParser
    {
        #region statics

        public static ConfigurationParser FromFile(string filename)
        {
            XmlDocument document = new XmlDocument();
            document.Load(filename);

            XmlNode structureMapNode = document.SelectSingleNode("//" + XmlConstants.STRUCTUREMAP);
            if (structureMapNode == null)
            {
                throw new StructureMapException(155, filename);
            }

            ConfigurationParser parser = new ConfigurationParser(structureMapNode);
            parser.FilePath = filename;
            
            return parser;
        }

        #endregion

        public string Description = string.Empty;

        private readonly XmlMementoCreator _mementoCreator;
        private readonly XmlNode _structureMapNode;
        private string _filePath = string.Empty;

        public ConfigurationParser(XmlNode structureMapNode)
        {
            _structureMapNode = structureMapNode;

            // TODO:  3.5 cleanup with extension method
            XmlMementoStyle mementoStyle = XmlMementoStyle.NodeNormalized;


            XmlAttribute att = _structureMapNode.Attributes[XmlConstants.MEMENTO_STYLE];
            if (att != null)
            {
                if (att.Value == XmlConstants.ATTRIBUTE_STYLE)
                {
                    mementoStyle = XmlMementoStyle.AttributeNormalized;
                }
            }


            _mementoCreator = new XmlMementoCreator(
                mementoStyle,
                XmlConstants.TYPE_ATTRIBUTE,
                XmlConstants.KEY_ATTRIBUTE);
        }

        public void ForEachFile(GraphLog log, Action<string> action)
        {
            // TODO:  Clean up with 3.5
            string includePath = getIncludePath();
            XmlNodeList includeNodes = _structureMapNode.SelectNodes(XmlConstants.INCLUDE_NODE);
            foreach (XmlElement includeElement in includeNodes)
            {
                string fileName = includeElement.GetAttribute("File");
                if (string.IsNullOrEmpty(fileName))
                {
                    log.RegisterError(156, _filePath);
                }
                else
                {
                    string includedFile = Path.Combine(includePath, fileName);
                    action(includedFile);
                }
            }
        }

        private string getIncludePath()
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                return string.Empty;
            }

            return Path.GetDirectoryName(_filePath);
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

        public void ParseAssemblies(IGraphBuilder builder)
        {
            parseAssemblies(builder);
        }


        public void ParseInstances(IGraphBuilder builder)
        {
            XmlNodeList familyNodes = findNodes(XmlConstants.PLUGIN_FAMILY_NODE);
            foreach (XmlElement familyElement in familyNodes)
            {
                TypePath typePath = TypePath.CreateFromXmlNode(familyElement);

                builder.ConfigureFamily(typePath,
                                        delegate(PluginFamily family) { attachInstances(family, familyElement, builder); });
            }
        }

        private void parseAssemblies(IGraphBuilder builder)
        {
            XmlNodeList assemblyNodes = findNodes(XmlConstants.ASSEMBLY);
            foreach (XmlNode assemblyNode in assemblyNodes)
            {
                string assemblyName = assemblyNode.Attributes[XmlConstants.NAME].Value;

                builder.AddAssembly(assemblyName);
            }
        }

        private XmlNodeList findNodes(string nodeName)
        {
            return _structureMapNode.SelectNodes(nodeName);
        }


        public void ParseFamilies(IGraphBuilder builder)
        {
            FamilyParser familyParser = new FamilyParser(builder, _mementoCreator);

            XmlNodeList familyNodes = findNodes(XmlConstants.PLUGIN_FAMILY_NODE);
            foreach (XmlElement familyElement in familyNodes)
            {
                familyParser.ParseFamily(familyElement);
            }

            XmlNodeList defaultNodes = findNodes(XmlConstants.DEFAULT_INSTANCE);
            foreach (XmlElement element in defaultNodes)
            {
                familyParser.ParseDefaultElement(element);
            }

            XmlNodeList instanceNodes = findNodes(XmlConstants.ADD_INSTANCE_NODE);
            foreach (XmlElement element in instanceNodes)
            {
                familyParser.ParseInstanceElement(element);
            }
        }


        private void attachInstances(PluginFamily family, XmlElement familyElement, IGraphBuilder builder)
        {
            foreach (XmlNode instanceNode in familyElement.ChildNodes)
            {
                if (instanceNode.Name != XmlConstants.INSTANCE_NODE)
                {
                    continue;
                }

                InstanceMemento memento = _mementoCreator.CreateMemento(instanceNode);
                family.AddInstance(memento);
            }
        }

        public void ParseProfilesAndMachines(IGraphBuilder builder)
        {
            ProfileAndMachineParser parser = new ProfileAndMachineParser(builder, _structureMapNode, _mementoCreator);
            parser.Parse();
        }
    }
}