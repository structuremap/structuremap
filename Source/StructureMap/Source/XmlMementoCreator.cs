using System;
using System.Xml;

namespace StructureMap.Source
{
    // TODO:  3.5, eliminate this with lambdas in ConfigurationParser
    public class XmlMementoCreator
    {
        private readonly Func<XmlNode, InstanceMemento> _createMemento;
        private readonly string _keyAttribute;
        private readonly string _typeAttribute;

        public XmlMementoCreator(XmlMementoStyle style, string typeAttribute, string keyAttribute)
        {
            _typeAttribute = typeAttribute;
            _keyAttribute = keyAttribute;
            if (style == XmlMementoStyle.NodeNormalized)
            {
                _createMemento = createNodeNormalizedMemento;
            }
            else
            {
                _createMemento = createAttributeNormalizedMemento;
            }
        }

        private InstanceMemento createNodeNormalizedMemento(XmlNode node)
        {
            XmlNode clonedNode = node.CloneNode(true);
            return new XmlNodeInstanceMemento(clonedNode, _typeAttribute, _keyAttribute);
        }


        private InstanceMemento createAttributeNormalizedMemento(XmlNode node)
        {
            XmlNode clonedNode = node.CloneNode(true);
            return new XmlAttributeInstanceMemento(clonedNode);
        }

        public InstanceMemento CreateMemento(XmlNode node)
        {
            return _createMemento(node);
        }
    }
}