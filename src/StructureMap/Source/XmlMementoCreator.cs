using System;
using System.Xml;

namespace StructureMap.Source
{
    [Obsolete("Not sure this is necessary now")]
    public class XmlMementoCreator
    {
        public InstanceMemento CreateMemento(XmlNode node)
        {
            XmlNode clonedNode = node.CloneNode(true);
            return new XmlAttributeInstanceMemento(clonedNode);
        }
    }
}