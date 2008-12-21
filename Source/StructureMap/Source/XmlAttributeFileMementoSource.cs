namespace StructureMap.Source
{
    /// <summary>
    /// Stores Attribute-normalized InstanceMemento's in an external file
    /// </summary>
    public class XmlAttributeFileMementoSource : XmlFileMementoSource
    {
        public XmlAttributeFileMementoSource(string filePath, string xpath, string nodeName)
            : base(filePath, xpath, nodeName, XmlMementoStyle.AttributeNormalized)
        {
        }
    }
}