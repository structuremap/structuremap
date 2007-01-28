using System;
using System.Collections;
using System.IO;
using System.Xml;
using StructureMap.Graph;
using StructureMap.Graph.Configuration;
using StructureMap.Source;

namespace StructureMap.Configuration
{
    public class ConfigurationParser
    {
        #region statics

        public static ConfigurationParser[] GetParsers(XmlDocument document, string includePath)
        {
            string folder = string.IsNullOrEmpty(includePath) ? string.Empty : Path.GetDirectoryName(includePath);

            ArrayList list = new ArrayList();
            list.Add(new ConfigurationParser(document.DocumentElement));

            string includedPath = null;

            try
            {
                XmlNodeList includeNodes = document.DocumentElement.SelectNodes(XmlConstants.INCLUDE_NODE);
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

        public static string[] GetDeploymentTargets(XmlNode node)
        {
            string[] returnValue = new string[0];

            XmlAttribute att = node.Attributes[XmlConstants.DEPLOYMENT_ATTRIBUTE];
            if (att != null)
            {
                string deployTargetArray = att.Value;

                deployTargetArray = deployTargetArray.Replace(" ,", ",");
                deployTargetArray = deployTargetArray.Replace(", ", ",");


                returnValue = deployTargetArray.Split(',');
            }

            return returnValue;
        }

        public static ConfigurationParser FromFile(string filename)
        {
            XmlDocument document = new XmlDocument();
            document.Load(filename);

            XmlNode structureMapNode = document.SelectSingleNode("//" + XmlConstants.STRUCTUREMAP);
            return new ConfigurationParser(structureMapNode);
        }

        #endregion

        private readonly XmlNode _structureMapNode;

        public ConfigurationParser(XmlNode structureMapNode)
        {
            _structureMapNode = structureMapNode;
        }

        public void ParseAssemblies(IGraphBuilder builder)
        {
            parseAssemblies(builder);
        }

        public void ParseFamilies(IGraphBuilder builder)
        {
            parseFamilies(builder);
        }

        public void ParseInstances(IGraphBuilder builder)
        {
            parseInstances(builder);

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
                string[] deploymentTargets = GetDeploymentTargets(assemblyNode);

                builder.AddAssembly(assemblyName, deploymentTargets);
            }
        }

        private XmlNodeList findNodes(string nodeName)
        {
            return _structureMapNode.SelectNodes(nodeName);
        }


        private void parseFamilies(IGraphBuilder builder)
        {
            FamilyParser familyParser = new FamilyParser(builder);
            XmlNodeList familyNodes = findNodes(XmlConstants.PLUGIN_FAMILY_NODE);
            foreach (XmlElement familyElement in familyNodes)
            {
                familyParser.ParseFamily(familyElement);
            }
        }


        private static void attachInstances(TypePath pluginTypePath, XmlElement familyElement, IGraphBuilder builder)
        {
            foreach (XmlNode instanceNode in familyElement.ChildNodes)
            {
                if (instanceNode.Name != XmlConstants.INSTANCE_NODE)
                {
                    continue;
                }

                XmlNodeInstanceMemento memento =
                    new XmlNodeInstanceMemento(instanceNode, XmlConstants.TYPE_ATTRIBUTE, XmlConstants.KEY_ATTRIBUTE);
                builder.RegisterMemento(pluginTypePath, memento);
            }
        }

        private void parseInstances(IGraphBuilder builder)
        {
            XmlNode instancesNode = _structureMapNode[XmlConstants.INSTANCES_NODE];
            if (instancesNode == null)
            {
                return;
            }

            foreach (XmlNode instanceNode in instancesNode)
            {
                string pluginTypeName = instanceNode.Name;
                TypePath typePath = TypePath.TypePathForFullName(pluginTypeName);

                XmlNodeInstanceMemento memento =
                    new XmlNodeInstanceMemento(instanceNode, XmlConstants.TYPE_ATTRIBUTE, XmlConstants.KEY_ATTRIBUTE);

                builder.RegisterMemento(typePath, memento);
            }
        }


        public void ParseProfilesAndMachines(IGraphBuilder builder)
        {
            XmlNode defaultProfileNode = _structureMapNode.Attributes.GetNamedItem(XmlConstants.DEFAULT_PROFILE);
            if (defaultProfileNode != null)
            {
                builder.DefaultManager.DefaultProfileName = defaultProfileNode.InnerText;
            }

            foreach (XmlElement profileElement in findNodes(XmlConstants.PROFILE_NODE))
            {
                string profileName = profileElement.GetAttribute(XmlConstants.NAME);
                builder.AddProfile(profileName);

                writeOverrides(profileElement,
                               delegate(string fullName, string defaultKey) { builder.OverrideProfile(fullName, defaultKey); });
            }

            foreach (XmlElement machineElement in findNodes(XmlConstants.MACHINE_NODE))
            {
                string machineName = machineElement.GetAttribute(XmlConstants.NAME);
                string profileName = machineElement.GetAttribute(XmlConstants.PROFILE_NODE);

                builder.AddMachine(machineName, profileName);

                writeOverrides(machineElement,
                               delegate(string fullName, string defaultKey) { builder.OverrideMachine(fullName, defaultKey); });
            }
        }

        private delegate void WriteOverride(string fullTypeName, string defaultKey);

        private static void writeOverrides(XmlElement parentElement, WriteOverride function)
        {
            foreach (XmlElement overrideElement in parentElement.SelectNodes(XmlConstants.OVERRIDE))
            {
                string fullName = overrideElement.GetAttribute(XmlConstants.TYPE_ATTRIBUTE);
                string defaultKey = overrideElement.GetAttribute(XmlConstants.DEFAULT_KEY_ATTRIBUTE);

                function(fullName, defaultKey);
            }
        }
    }
}