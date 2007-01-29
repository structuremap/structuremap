using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace StructureMap.DeploymentTasks
{
    [TaskName("structuremap.removeassembly")]
    public class RemoveAssemblyTask : Task
    {
        private string _configPath;
        private string _assemblyName;

        public RemoveAssemblyTask() : base()
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
            Log(Level.Debug, message);

            XmlDocument document = new XmlDocument();
            document.Load(_configPath);

            string xpath = string.Format("Assembly[@Name='{0}']", _assemblyName);
            XmlNode node = document.DocumentElement.SelectSingleNode(xpath);
            if (node != null)
            {
                Log(Level.Debug, "Found the Assembly node");
                node.ParentNode.RemoveChild(node);
            }

            document.Save(_configPath);
        }
    }
}