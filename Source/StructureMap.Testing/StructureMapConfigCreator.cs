using System;
using System.Xml;
using StructureMap.Configuration;

namespace StructureMap.Testing
{
    public class StructureMapConfigCreator
    {
        private readonly XmlDocument _document;
        private readonly XmlNode _root;
        private XmlElement _lastElement;
        private XmlElement _lastFamily;

        public StructureMapConfigCreator()
        {
            _document = new XmlDocument();
            _root = _document.CreateElement(XmlConstants.STRUCTUREMAP);
            _document.AppendChild(_root);
        }

        public void AddFamily(Type pluginType)
        {
            XmlElement familyElement = _document.CreateElement(XmlConstants.PLUGIN_FAMILY_NODE);
            familyElement.SetAttribute(XmlConstants.ASSEMBLY, pluginType.Assembly.GetName().Name);
            familyElement.SetAttribute(XmlConstants.TYPE_ATTRIBUTE, pluginType.Name);

            _root.AppendChild(familyElement);
            _lastFamily = _lastElement = familyElement;
        }
    }
}