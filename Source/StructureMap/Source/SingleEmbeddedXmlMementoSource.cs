using System.IO;
using System.Reflection;
using System.Xml;

namespace StructureMap.Source
{
    public class SingleEmbeddedXmlMementoSource : XmlMementoSource
    {
        private readonly Assembly _assembly;
        private readonly string _path;

        /// <summary>
        /// Retrieves Xml InstanceMemento's from an xml file stored as an embedded resource in an assembly.
        /// </summary>
        /// <param name="nodeName">Designates the nodes that are memento nodes</param>
        /// <param name="style">NodeNormalized or AttributeNormalized</param>
        /// <param name="assemblyName">The name of the Assembly the file is embedded into</param>
        /// <param name="path">The path to the embedded resource within the file</param>
        [DefaultConstructor]
        public SingleEmbeddedXmlMementoSource(string nodeName, XmlMementoStyle style, string assemblyName, string path)
            : base(nodeName, "Type", "Key", style)
        {
            _path = path;
            _assembly = Assembly.Load(assemblyName);
        }

        public SingleEmbeddedXmlMementoSource(string nodeName, XmlMementoStyle style, Assembly assembly, string path)
            : base(nodeName, "Type", "Key", style)
        {
            _assembly = assembly;
            _path = path;
        }


        public override string Description
        {
            get { return "Single Embedded File:  " + _path; }
        }

        protected override XmlNode getRootNode()
        {
            Stream stream = _assembly.GetManifestResourceStream(_path);
            var document = new XmlDocument();
            document.Load(stream);

            return document.DocumentElement;
        }
    }
}