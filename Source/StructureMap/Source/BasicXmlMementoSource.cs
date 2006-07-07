using System.Xml;

namespace StructureMap.Source
{
	/// <summary>
	/// Generic implementation of an XmlMementoSource
	/// </summary>
	public class BasicXmlMementoSource : XmlMementoSource
	{
		private XmlNode _Node;

		public BasicXmlMementoSource(XmlNode Node, string NodeName) : base(NodeName, "Type", "Key", XmlMementoStyle.NodeNormalized)
		{
			_Node = Node;
		}

		protected override XmlNode getRootNode()
		{
			return _Node;
		}

		public override string Description
		{
			get { return "BasicXmlMementoSource"; }
		}
	}
}