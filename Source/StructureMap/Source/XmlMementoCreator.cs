using System.Xml;

namespace StructureMap.Source
{
	public delegate InstanceMemento CreateXmlMementoDelegate(XmlNode node);

	public class XmlMementoCreator
	{
		private readonly string _typeAttribute;
		private readonly string _keyAttribute;
		private CreateXmlMementoDelegate _createMemento;

		public XmlMementoCreator(XmlMementoStyle style, string typeAttribute, string keyAttribute)
		{
			_typeAttribute = typeAttribute;
			_keyAttribute = keyAttribute;
			if (style == XmlMementoStyle.NodeNormalized)
			{
				_createMemento = new CreateXmlMementoDelegate(this.createNodeNormalizedMemento);
			}
			else
			{
				_createMemento = new CreateXmlMementoDelegate(this.createAttributeNormalizedMemento);
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
