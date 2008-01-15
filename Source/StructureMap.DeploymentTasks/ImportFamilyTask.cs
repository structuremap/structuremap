using System;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;
using StructureMap.Configuration;

namespace StructureMap.DeploymentTasks
{
    [TaskName("structuremap.importfamily")]
    public class ImportFamilyTask : Task
    {
        private string _pluginType;
        private string _sourcePath;
        private string _targetPath;

        public ImportFamilyTask() : base()
        {
        }

        [TaskAttribute("targetPath", Required=true)]
        public string TargetPath
        {
            get { return _targetPath; }
            set { _targetPath = value; }
        }

        [TaskAttribute("sourcePath", Required=true)]
        public string SourcePath
        {
            get { return _sourcePath; }
            set { _sourcePath = value; }
        }

        [TaskAttribute("pluginType", Required=true)]
        public string PluginType
        {
            get { return _pluginType; }
            set { _pluginType = value; }
        }

        protected override void ExecuteTask()
        {
            string message =
                string.Format("Importing PluginFamily '{0}' from {1} to {2}", _pluginType, _sourcePath, _targetPath);
            Console.WriteLine(message);

            XmlDocument targetXml = new XmlDocument();
            targetXml.Load(_targetPath);

            XmlDocument sourceXml = new XmlDocument();
            sourceXml.Load(_sourcePath);

            ImportFamilyNode(sourceXml, targetXml);

            targetXml.Save(_targetPath);
        }

        public void ImportFamilyNode(XmlDocument sourceXml, XmlDocument targetXml)
        {
            XmlNode familyNode = getFamilyNode(sourceXml);

            XmlNode importedFamilyNode = targetXml.ImportNode(familyNode, true);
            targetXml.DocumentElement.AppendChild(importedFamilyNode);

            addAssemblyNode(importedFamilyNode, targetXml);
        }

        private XmlNode getFamilyNode(XmlDocument sourceXml)
        {
            string message;
            string xpath =
                string.Format("//{0}[@{1}='{2}']", XmlConstants.PLUGIN_FAMILY_NODE, XmlConstants.TYPE_ATTRIBUTE,
                              _pluginType);
            XmlNode familyNode = sourceXml.DocumentElement.SelectSingleNode(xpath);
            if (familyNode == null)
            {
                message = string.Format("PluginFamily {0} was not found in {1}", _pluginType, _sourcePath);
                throw new ApplicationException(message);
            }
            return familyNode;
        }

        private void addAssemblyNode(XmlNode importedFamilyNode, XmlDocument targetXml)
        {
            // Insure the Assembly node exists
            string assemblyName = importedFamilyNode.Attributes[XmlConstants.ASSEMBLY].InnerText;
            AddAssembly addAssemblyTask = new AddAssembly();
            addAssemblyTask.AssemblyName = assemblyName;
            addAssemblyTask.AddAssemblyToDocument(targetXml);
        }
    }
}