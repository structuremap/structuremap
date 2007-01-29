using System;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace StructureMap.DeploymentTasks
{
    [TaskName("structuremap.addassembly")]
    public class AddAssembly : Task
    {
        private string _configPath;
        private string _assemblyName;

        public AddAssembly()
        {
        }

        [TaskAttribute("configPath", Required=true)]
        public string ConfigPath
        {
            get { return _configPath; }
            set { _configPath = value; }
        }

        [TaskAttribute("assemblyName", Required=true)]
        public string AssemblyName
        {
            get { return _assemblyName; }
            set { _assemblyName = value; }
        }

        protected override void ExecuteTask()
        {
            string message = string.Format("Adding Assembly {0} to configuration file {1}", _assemblyName, _configPath);
            Console.WriteLine(message);

            XmlDocument document = AddAssemblyNode();

            document.Save(_configPath);
        }

        public XmlDocument AddAssemblyNode()
        {
            XmlDocument document = new XmlDocument();
            document.Load(_configPath);

            AddAssemblyToDocument(document);

            return document;
        }

        public void AddAssemblyToDocument(XmlDocument document)
        {
            string xpath = string.Format("Assembly[@Name='{0}']", _assemblyName);
            XmlNode node = document.DocumentElement.SelectSingleNode(xpath);
            if (node == null)
            {
                XmlElement element = document.CreateElement("Assembly");
                element.SetAttribute("Name", _assemblyName);
                if (!document.DocumentElement.HasChildNodes)
                {
                    document.DocumentElement.AppendChild(element);
                }
                else
                {
                    document.DocumentElement.InsertBefore(element, document.DocumentElement.FirstChild);
                }
            }
        }
    }
}