using System.Xml;
using StructureMap.Graph;

namespace StructureMap.XmlMapping
{
    public class ConfigEditor
    {
        private readonly XmlDocument _document;

        public ConfigEditor(XmlDocument document)
        {
            _document = document;
        }

        public void RemoveAssembly(string assemblyName)
        {
            string xpath1 = string.Format("Assembly[@Name='{0}']", assemblyName);
            removeNodes(xpath1);

            string xpath2 = string.Format("PluginFamily[@Assembly='{0}']", assemblyName);
            removeNodes(xpath2);

            string xpath3 = string.Format("Plugin[@Assembly='{0}']", assemblyName);
            removeNodes(xpath3);
        }

        public void RemovePluginFamily(string pluginTypeName)
        {
            string xpath1 = string.Format("PluginFamily[@Type='{0}']", pluginTypeName);
            removeNodes(xpath1);

            string xpath2 = "Instances/" + pluginTypeName;
            removeNodes(xpath2);

            string xpath3 = string.Format("//Override[@Type='{0}']", pluginTypeName);
            removeNodes(xpath3);
        }

        public void RemovePlugin(TypePath pluginType, string concreteKey)
        {
            string pluginTypeFullName = pluginType.ClassName;
            string xpath1 =
                string.Format("PluginFamily[@Type='{0}']/Plugin[@ConcreteKey='{1}']", pluginTypeFullName, concreteKey);
            removeNodes(xpath1);

            string xpath2 =
                string.Format("PluginFamily[@Type='{0}']/Instance[@Type='{1}']", pluginTypeFullName, concreteKey);
            removeNodes(xpath2);

            string xpath3 = string.Format("//{0}[@Type='{1}']", pluginTypeFullName, concreteKey);
            removeNodes(xpath3);
        }


        private void removeNodes(string xpath)
        {
            XmlNodeList nodes = _document.DocumentElement.SelectNodes(xpath);
            foreach (XmlNode node in nodes)
            {
                node.ParentNode.RemoveChild(node);
            }
        }

        public void FilterProfileToDefault(string profileName)
        {
            _document.DocumentElement.SetAttribute("DefaultProfile", profileName);

            string xpath = string.Format("Profile[@Name='{0}']", profileName);
            XmlNode profileNode = _document.DocumentElement.SelectSingleNode(xpath);

            XmlNodeList overrideNodes = profileNode.SelectNodes("Override");
            foreach (XmlElement element in overrideNodes)
            {
                string pluginType = element.GetAttribute("Type");
                string instanceKey = element.GetAttribute("DefaultKey");

                FilterToDefaultInstance(pluginType, instanceKey);
            }

            string profileXPath = string.Format("Profile[@Name!='{0}']", profileName);
            removeNodes(profileXPath);
        }

        public void CreateDefaultProfile(Profile profile)
        {
            removeNodes("Profile");

            const string PROFILE_NAME = "DEFAULT";

            _document.DocumentElement.SetAttribute("DefaultProfile", PROFILE_NAME);

            XmlElement profileElement = _document.CreateElement("Profile");
            profileElement.SetAttribute("Name", PROFILE_NAME);
            _document.DocumentElement.InsertBefore(profileElement, _document.DocumentElement.FirstChild);

            foreach (InstanceDefault instanceDefault in profile.Defaults)
            {
                XmlElement overrideElement = _document.CreateElement("Override");

                overrideElement.SetAttribute("Type", instanceDefault.PluginTypeName);
                overrideElement.SetAttribute("DefaultKey", instanceDefault.DefaultKey);

                FilterToDefaultInstance(instanceDefault.PluginTypeName, instanceDefault.DefaultKey);

                profileElement.AppendChild(overrideElement);
            }
        }

        public void FilterToDefaultInstance(string pluginType, string instanceKey)
        {
            string xpath = string.Format("PluginFamily[@Type='{0}']", pluginType);
            XmlElement familyElement = (XmlElement) _document.DocumentElement.SelectSingleNode(xpath);


            if (familyElement != null)
            {
                familyElement.SetAttribute("DefaultKey", instanceKey);
            }

            string xpath1 = string.Format("PluginFamily[@Type='{0}']/Instance[@Key!='{1}']", pluginType, instanceKey);
            removeNodes(xpath1);

            string xpath2 = string.Format("Instances/{0}[@Key!='{1}']", pluginType, instanceKey);
            removeNodes(xpath2);
        }

        public void RemoveAllMachineOptions()
        {
            removeNodes("Machine");
        }
    }
}