using System;
using System.Collections.Generic;
using System.Xml;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Source
{
    /// <summary>
    /// Implementation of InstanceMemento that stores information in a node-normalized
    /// Xml format.
    /// </summary>
    public class XmlNodeInstanceMemento : InstanceMemento
    {
        private readonly XmlNode _innerNode;
        private readonly string _keyAttribute;
        private readonly string _typeAttribute;

        public XmlNodeInstanceMemento(XmlNode Node, string TypeAttribute, string KeyAttribute)
        {
            if (Node == null)
            {
                throw new ArgumentNullException("Node");
            }

            _innerNode = Node;
            _typeAttribute = TypeAttribute;
            _keyAttribute = KeyAttribute;
        }

        protected override string innerConcreteKey { get { return getAttribute(_typeAttribute); } }

        protected override string innerInstanceKey { get { return getAttribute(_keyAttribute); } }

        public override bool IsReference
        {
            get
            {
                if (!string.IsNullOrEmpty(getTPluggedType()))
                {
                    return false;
                }

                bool returnValue = false;

                string typeName = getAttribute("Type");

                // If a TypeName is not specified, then "true"
                if (typeName == string.Empty)
                {
                    returnValue = true;
                }

                return returnValue;
            }
        }

        public override string ReferenceKey { get { return getAttribute("Key"); } }


        protected override string getTPluggedType()
        {
            return getAttribute(XmlConstants.PLUGGED_TYPE);
        }

        private XmlElement getChildNode(string Key)
        {
            string xpath = string.Format("Property[@Name='{0}']", Key);
            var nodeProperty = (XmlElement) _innerNode.SelectSingleNode(xpath);

            return nodeProperty;
        }


        protected override string getPropertyValue(string Key)
        {
            XmlNode nodeProperty = getChildNode(Key);
            if (nodeProperty == null)
            {
                return null;
            }

            if (nodeProperty.InnerText != string.Empty)
            {
                return nodeProperty.InnerText.Trim();
            }
            else
            {
                return nodeProperty.Attributes["Value"].Value;
            }
        }


        public override Instance ReadChildInstance(string name, PluginGraph graph, Type childType)
        {
            ITypeReader reader = TypeReaderFactory.GetReader(childType);
            if (reader == null)
            {
                return base.ReadChildInstance(name, graph, childType);
            }

            XmlElement element = getChildNode(name);
            return element == null ? null : reader.Read(element, childType);
        }

        protected override InstanceMemento getChild(string Key)
        {
            InstanceMemento returnValue = null;

            XmlNode nodeChild = getChildNode(Key);

            if (nodeChild != null)
            {
                returnValue = new XmlNodeInstanceMemento(nodeChild, _typeAttribute, _keyAttribute);
            }

            return returnValue;
        }

        private string getAttribute(string AttributeName)
        {
            string returnValue = string.Empty;
            XmlAttribute att = _innerNode.Attributes[AttributeName];
            if (att != null)
            {
                returnValue = att.Value;
            }

            return returnValue;
        }


        public override InstanceMemento[] GetChildrenArray(string Key)
        {
            XmlNode nodeChild = getChildNode(Key);
            if (nodeChild == null)
            {
                return null;
            }

            var list = new List<InstanceMemento>();
            foreach (XmlNode childNode in nodeChild.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    list.Add(new XmlNodeInstanceMemento(childNode, _typeAttribute, _keyAttribute));
                }
            }

            return list.ToArray();
        }

        public override string ToString()
        {
            return _innerNode.OuterXml;
        }
    }
}