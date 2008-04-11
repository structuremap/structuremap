using System;
using System.Collections.Generic;
using System.Xml;
using StructureMap.Configuration;
using StructureMap.Pipeline;

namespace StructureMap.Source
{
    /// <summary>
    /// Implementation of InstanceMemento that stores information in a node-normalized
    /// Xml format.
    /// </summary>
    public class XmlNodeInstanceMemento : InstanceMemento
    {
        private XmlNode _innerNode;
        private string _keyAttribute;
        private string _typeAttribute;

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

        protected override string innerConcreteKey
        {
            get { return getAttribute(_typeAttribute); }
        }

        protected override string innerInstanceKey
        {
            get { return getAttribute(_keyAttribute); }
        }

        public override bool IsReference
        {
            get
            {
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

        public override string ReferenceKey
        {
            get { return getAttribute("Key"); }
        }


        protected override string getPluggedType()
        {
            return getAttribute(XmlConstants.PLUGGED_TYPE);
        }

        private XmlNode getChildNode(string Key)
        {
            string xpath = string.Format("Property[@Name='{0}']", Key);
            XmlNode nodeProperty = _innerNode.SelectSingleNode(xpath);

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

        // TODO -- pull up into abstract class?


        public override InstanceMemento[] GetChildrenArray(string Key)
        {
            XmlNode nodeChild = getChildNode(Key);
            if (nodeChild == null)
            {
                return null;
            }

            List<InstanceMemento> list = new List<InstanceMemento>();
            foreach (XmlNode childNode in nodeChild.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    list.Add(new XmlNodeInstanceMemento(childNode, _typeAttribute, _keyAttribute));
                }
            }

            return list.ToArray();
        }

        public override InstanceMemento Substitute(InstanceMemento memento)
        {
            XmlTemplater templater = new XmlTemplater(_innerNode);
            XmlNode substitutedNode = templater.SubstituteTemplates(_innerNode, memento);
            return new XmlNodeInstanceMemento(substitutedNode, _typeAttribute, _keyAttribute);
        }

        public override string ToString()
        {
            return _innerNode.OuterXml;
        }
    }
}