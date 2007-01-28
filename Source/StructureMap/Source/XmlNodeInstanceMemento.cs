using System.Xml;
using StructureMap.Configuration.Tokens;

namespace StructureMap.Source
{
    /// <summary>
    /// Implementation of InstanceMemento that stores information in a node-normalized
    /// Xml format.
    /// </summary>
    public class XmlNodeInstanceMemento : InstanceMemento
    {
        private XmlNode _innerNode;
        private string _typeAttribute;
        private string _keyAttribute;

        public XmlNodeInstanceMemento(XmlNode Node, string TypeAttribute, string KeyAttribute)
        {
            _innerNode = Node;
            _typeAttribute = TypeAttribute;
            _keyAttribute = KeyAttribute;
        }

        #region InstanceMemento Members

        public override string ConcreteKey
        {
            get { return getAttribute(_typeAttribute); }
        }

        public override string InstanceKey
        {
            get { return getAttribute(_keyAttribute); }
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


        public override InstanceMemento[] GetChildrenArray(string Key)
        {
            XmlNode nodeChild = getChildNode(Key);
            if (nodeChild == null)
            {
                return null;
            }

            InstanceMemento[] returnValue = new InstanceMemento[nodeChild.ChildNodes.Count];
            for (int i = 0; i < returnValue.Length; i++)
            {
                returnValue[i] = new XmlNodeInstanceMemento(nodeChild.ChildNodes[i], _typeAttribute, _keyAttribute);
            }

            return returnValue;
        }

        public override InstanceMemento Substitute(InstanceMemento memento)
        {
            XmlTemplater templater = new XmlTemplater(_innerNode);
            XmlNode substitutedNode = templater.SubstituteTemplates(_innerNode, memento);
            return new XmlNodeInstanceMemento(substitutedNode, _typeAttribute, _keyAttribute);
        }

        public override TemplateToken CreateTemplateToken()
        {
            TemplateToken token = new TemplateToken();
            token.ConcreteKey = ConcreteKey;
            token.TemplateKey = InstanceKey;
            XmlTemplater templater = new XmlTemplater(_innerNode);

            token.Properties = templater.Substitutions;

            return token;
        }

        #endregion

        public override string ToString()
        {
            return _innerNode.OuterXml;
        }
    }
}