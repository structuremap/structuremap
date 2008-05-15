using System.Xml;

namespace StructureMap.Source
{
    /// <summary>
    /// Generic implementation of an XmlMementoSource
    /// </summary>
    public class BasicXmlMementoSource : XmlMementoSource
    {
        private readonly XmlNode _node;

        public BasicXmlMementoSource(XmlNode Node, string NodeName)
            : base(NodeName, "Type", "Key", XmlMementoStyle.NodeNormalized)
        {
            _node = Node;
        }

        public override string Description
        {
            get { return "BasicXmlMementoSource"; }
        }

        protected override XmlNode getRootNode()
        {
            return _node;
        }
    }
}