using System;
using System.Collections;
using System.IO;
using System.Xml;
using StructureMap.Graph;
using StructureMap.Source;

namespace StructureMap.Configuration
{
    public class ConfigurationParser
    {
        #region statics

        public static ConfigurationParser[] GetParsers(XmlDocument document, string includePath)
        {
            XmlElement node = document.DocumentElement;

            return GetParsers(node, includePath);
        }

        public static ConfigurationParser[] GetParsers(XmlNode node, string includePath)
        {
            string folder = string.IsNullOrEmpty(includePath) ? string.Empty : Path.GetDirectoryName(includePath);

            ArrayList list = new ArrayList();

            list.Add(new ConfigurationParser(node));

            string includedPath = null;

            try
            {
                XmlNodeList includeNodes = node.SelectNodes(XmlConstants.INCLUDE_NODE);
                foreach (XmlElement includeElement in includeNodes)
                {
                    XmlDocument includedDoc = new XmlDocument();
                    string fileName = includeElement.GetAttribute("File");

                    if (fileName == string.Empty)
                    {
                        throw new ApplicationException("The File attribute on the Include node is required");
                    }

                    try
                    {
                        includedPath = Path.Combine(folder, fileName);
                        includedDoc.Load(includedPath);

                        ConfigurationParser parser = new ConfigurationParser(includedDoc.DocumentElement);
                        list.Add(parser);
                    }
                    catch (Exception ex)
                    {
                        throw new StructureMapException(150, ex, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new StructureMapException(100, includedPath, ex);
            }

            return (ConfigurationParser[]) list.ToArray(typeof (ConfigurationParser));
        }


        public static ConfigurationParser FromFile(string filename)
        {
            XmlDocument document = new XmlDocument();
            document.Load(filename);

            XmlNode structureMapNode = document.SelectSingleNode("//" + XmlConstants.STRUCTUREMAP);
            return new ConfigurationParser(structureMapNode);
        }

        #endregion

        private readonly XmlMementoCreator _mementoCreator;
        private readonly XmlNode _structureMapNode;

        public ConfigurationParser(XmlNode structureMapNode)
        {
            _structureMapNode = structureMapNode;
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

        public string Id
        {
            get
            {
                XmlAttribute att = _structureMapNode.Attributes["Id"];
                return att == null ? string.Empty : att.Value;
            }
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
                attachInstances(typePath, familyElement, builder);
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


        private void attachInstances(TypePath pluginTypePath, XmlElement familyElement, IGraphBuilder builder)
        {
            foreach (XmlNode instanceNode in familyElement.ChildNodes)
            {
                if (instanceNode.Name != XmlConstants.INSTANCE_NODE)
                {
                    continue;
                }

                InstanceMemento memento = _mementoCreator.CreateMemento(instanceNode);
                builder.RegisterMemento(pluginTypePath, memento);
            }
        }

        public void ParseProfilesAndMachines(IGraphBuilder builder)
        {
            ProfileAndMachineParser parser = new ProfileAndMachineParser(builder, _structureMapNode, _mementoCreator);
            parser.Parse();
        }
    }
}