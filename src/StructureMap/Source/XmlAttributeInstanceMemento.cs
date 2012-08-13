using System;
using System.Collections;
using System.Xml;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Source
{
    /// <summary>
    /// An implementation of InstanceMemento that stores properties as Xml attributes
    /// Limited functionality
    /// </summary>
    public class XmlAttributeInstanceMemento : InstanceMemento
    {
        private readonly XmlElement _element;

        public XmlAttributeInstanceMemento(XmlNode node)
        {
            _element = (XmlElement) node;
        }

        protected override string innerConcreteKey { get { return _element.GetAttribute(XmlConstants.TYPE_ATTRIBUTE); } }

        protected override string innerInstanceKey { get { return _element.GetAttribute(XmlConstants.KEY_ATTRIBUTE); } }

        public XmlElement InnerElement { get { return _element; } }

        public override bool IsReference { get { return (ConcreteKey == string.Empty && string.IsNullOrEmpty(getPluggedType())); } }

        public override string ReferenceKey { get { return InstanceKey; } }

        public void SetConcreteKey(string concreteKey)
        {
            _element.SetAttribute(XmlConstants.TYPE_ATTRIBUTE, concreteKey);
        }

        protected override InstanceMemento getChild(string key)
        {
            XmlNode childNode = _element[key];
            if (childNode == null)
            {
                return null;
            }
            else
            {
                return new XmlAttributeInstanceMemento(childNode);
            }
        }

        protected override string getPropertyValue(string key)
        {
            if (_element.HasAttribute(key))
            {
                return _element.GetAttribute(key);
            }
            else
            {
                XmlElement childElement = _element[key];
                return childElement == null ? string.Empty : childElement.InnerText.Trim();
            }
        }

        public override InstanceMemento[] GetChildrenArray(string Key)
        {
            XmlNode childrenNode = _element[Key];
            if (childrenNode == null)
            {
                return null;
            }

            var list = new ArrayList();
            var element = (XmlElement) childrenNode.FirstChild;
            while (element != null)
            {
                if (element.NodeType == XmlNodeType.Element)
                {
                    InstanceMemento childMemento = new XmlAttributeInstanceMemento(element);
                    list.Add(childMemento);
                }

                element = (XmlElement) element.NextSibling;
            }

            return (InstanceMemento[]) list.ToArray(typeof (InstanceMemento));
        }

        public override InstanceMemento Substitute(InstanceMemento memento)
        {
            var templater = new XmlTemplater(_element);
            XmlNode substitutedNode = templater.SubstituteTemplates(_element, memento);

            return new XmlAttributeInstanceMemento(substitutedNode);
        }


        public override string ToString()
        {
            return _element.OuterXml;
        }

        public override Instance ReadChildInstance(string name, PluginGraph graph, Type childType)
        {
            ITypeReader reader = TypeReaderFactory.GetReader(childType);
            if (reader == null)
            {
                return base.ReadChildInstance(name, graph, childType);
            }

            XmlElement element = _element[name];
            return element == null ? null : reader.Read(element, childType);
        }
    }
}