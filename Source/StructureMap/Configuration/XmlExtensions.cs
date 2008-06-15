using System;
using System.Xml;

namespace StructureMap.Configuration
{
    public static class XmlExtensions
    {
        public static XmlTextExpression ForTextInChild(this XmlNode node, string xpath)
        {
            return new XmlTextExpression(node, xpath);
        }

        public static XmlNodeExpression ForEachChild(this XmlNode node, string xpath)
        {
            return new XmlNodeExpression(node, xpath);
        }

        public static void ForAttributeValue(this XmlNode node, string attributeName, Action<string> action)
        {
            XmlNode attNode = node.Attributes.GetNamedItem(attributeName);
            if (attNode != null)
            {
                action(attNode.InnerText);
            }
        }

        public static HasXmlElementExpression IfHasNode(this XmlNode node, string xpath)
        {
            return new HasXmlElementExpression(node, xpath);
        }

        public class HasXmlElementExpression
        {
            private XmlElement _element;

            internal HasXmlElementExpression(XmlNode parent, string xpath)
            {
                _element = (XmlElement)parent.SelectSingleNode(xpath);
            }

            public HasXmlElementExpression Do(Action<XmlElement> action)
            {
                if (_element != null)
                {
                    action(_element);
                }

                return this;
            }

            public void Else(Action action)
            {
                if (_element == null)
                {
                    action();
                }
            }
        }


        public class XmlNodeExpression
        {
            private XmlNodeList _list;

            internal XmlNodeExpression(XmlNode parent, string xpath)
            {
                _list = parent.SelectNodes(xpath);
            }

            public void Do(Action<XmlElement> action)
            {
                if (_list == null) return;

                foreach (XmlNode node in _list)
                {
                    action((XmlElement)node);
                }
            }
        }

        public class XmlTextExpression
        {
            private readonly XmlNodeList _list;

            internal XmlTextExpression(XmlNode parent, string attributePath)
            {
                _list = parent.SelectNodes(attributePath);
            }

            public void Do(Action<string> action)
            {
                if (_list == null) return;

                foreach (XmlNode node in _list)
                {
                    action(node.InnerText);
                }
            }
        }


    }
}