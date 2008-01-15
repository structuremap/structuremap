using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace StructureMap.DeploymentTasks
{
    /// <summary>
    /// Attempts to do an Xml merge of two StructureMap configuration files
    /// </summary>
    [TaskName("structuremap.dumbmerge")]
    public class DumbConfigMergeTask : Task
    {
        private string _destinationPath;
        private string _mainConfig;
        private string _secondConfig;

        public DumbConfigMergeTask()
        {
        }

        /// <summary>
        /// Target configuration file to receive
        /// </summary>
        [TaskAttribute("main", Required=true)]
        public string MainConfig
        {
            get { return _mainConfig; }
            set { _mainConfig = value; }
        }

        [TaskAttribute("second", Required=true)]
        public string SecondConfig
        {
            get { return _secondConfig; }
            set { _secondConfig = value; }
        }

        [TaskAttribute("destination", Required=true)]
        public string DestinationPath
        {
            get { return _destinationPath; }
            set { _destinationPath = value; }
        }

        protected override void ExecuteTask()
        {
            XmlDocument document = new XmlDocument();
            document.Load(_mainConfig);

            XmlDocument secondaryDocument = new XmlDocument();
            secondaryDocument.Load(_secondConfig);

            addAssemblies(secondaryDocument, document);
            addFamilies(secondaryDocument, document);
            addOverrides(secondaryDocument, document);
            addInstances(document, secondaryDocument);

            document.Save(_destinationPath);
        }

        private static void addInstances(XmlDocument document, XmlDocument secondaryDocument)
        {
            XmlNode instancesNode = document.DocumentElement["Instances"];
            if (instancesNode == null)
            {
                instancesNode = document.CreateElement("Instances");
                document.DocumentElement.AppendChild(instancesNode);
            }

            foreach (XmlNode instanceNode in secondaryDocument.DocumentElement.SelectNodes("//Instance"))
            {
                XmlNode newInstanceNode = document.ImportNode(instanceNode, true);
                instancesNode.AppendChild(newInstanceNode);
            }
        }

        private void addOverrides(XmlDocument secondaryDocument, XmlDocument document)
        {
            SetOverrideTask setOverrideCommand = new SetOverrideTask();
            foreach (XmlElement profileElement in secondaryDocument.DocumentElement.SelectNodes("Profile"))
            {
                setOverrideCommand.ProfileName = profileElement.GetAttribute("Name");
                foreach (XmlElement overrideElement in profileElement.ChildNodes)
                {
                    setOverrideCommand.TypeName = overrideElement.GetAttribute("Type");
                    setOverrideCommand.DefaultKey = overrideElement.GetAttribute("DefaultKey");
                    setOverrideCommand.ApplyToDocument(document);
                }
            }
        }

        private void addFamilies(XmlDocument secondaryDocument, XmlDocument document)
        {
            foreach (XmlNode familyNode in secondaryDocument.DocumentElement.SelectNodes("//PluginFamily"))
            {
                XmlNode newFamilyNode = document.ImportNode(familyNode, true);
                document.DocumentElement.AppendChild(newFamilyNode);
            }
        }

        private void addAssemblies(XmlDocument secondaryDocument, XmlDocument document)
        {
            AddAssembly addAssemblyTask = new AddAssembly();
            XmlNodeList assemblyNodes = secondaryDocument.DocumentElement.SelectNodes("//Assembly");
            foreach (XmlElement assemblyElement in assemblyNodes)
            {
                addAssemblyTask.AssemblyName = assemblyElement.GetAttribute("Name");
                addAssemblyTask.AddAssemblyToDocument(document);
            }
        }
    }
}