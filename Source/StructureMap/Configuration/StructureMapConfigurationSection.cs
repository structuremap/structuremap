using System.Configuration;
using System.Xml;

namespace StructureMap.Configuration
{
    public class StructureMapConfigurationSection : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            XmlNode parentNode = parent as XmlNode;
            if (parentNode == null) return section;
            // Might need to make this more intelligent, to merge nodes that override eachother
            foreach (XmlNode childNode in section.ChildNodes)
            {
                XmlNode importedNode = parentNode.OwnerDocument.ImportNode(childNode, true);
                parentNode.AppendChild(importedNode);
            }
            return parentNode;
        }

        public static XmlNode GetStructureMapConfiguration()
        {
            XmlNode node = ConfigurationSettings.GetConfig(XmlConstants.STRUCTUREMAP) as XmlNode;
            if (node == null)
            {
                throw new StructureMapException(105, XmlConstants.STRUCTUREMAP);
            }
            return node;
        }
    }
}