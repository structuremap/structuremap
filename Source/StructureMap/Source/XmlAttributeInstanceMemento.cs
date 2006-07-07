using System;
using System.Collections;
using System.Reflection;
using System.Xml;
using StructureMap.Configuration.Tokens;

namespace StructureMap.Source
{
	/// <summary>
	/// An implementation of InstanceMemento that stores properties as Xml attributes
	/// Limited functionality
	/// </summary>
	public class XmlAttributeInstanceMemento : InstanceMemento
	{
		private XmlElement _element;

		public XmlAttributeInstanceMemento(XmlNode node)
		{
			_element = (XmlElement) node;
		}

		public override string ConcreteKey
		{
			get { return _element.GetAttribute("Type"); }
		}

		public void SetConcreteKey(string concreteKey)
		{
			_element.SetAttribute("Type", concreteKey);
		}

		public override string InstanceKey
		{
			get { return _element.GetAttribute("Key"); }
		}

		public XmlElement InnerElement
		{
			get { return _element; }
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

		public override bool IsReference
		{
			get { return (this.ConcreteKey == string.Empty); }
		}

		public override string ReferenceKey
		{
			get { return this.InstanceKey; }
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

			ArrayList list = new ArrayList();
			XmlElement element = (XmlElement) childrenNode.FirstChild;
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
			XmlTemplater templater = new XmlTemplater(_element);
			XmlNode substitutedNode = templater.SubstituteTemplates(_element, memento);

			return new XmlAttributeInstanceMemento(substitutedNode);
		}

		public override TemplateToken CreateTemplateToken()
		{
			try
			{
				TemplateToken token = new TemplateToken();
				token.ConcreteKey = this.ConcreteKey;
				token.TemplateKey = this.InstanceKey;
				XmlTemplater templater = new XmlTemplater(_element);

				token.Properties = templater.Substitutions;

				return token;
			}
			catch (Exception ex)
			{
				string message = "Error creating a TemplateToken for " + _element.OuterXml;
				throw new ApplicationException(message, ex);
			}
		}

		public override string ToString()
		{
			return _element.OuterXml;
		}

	}
}