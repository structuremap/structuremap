using System;
using System.IO;
using System.Xml;

namespace StructureMap.Source
{
    /// <summary>
    /// Implementation of XmlMementoSource that reads InstanceMemento's from an external file.
    /// Useful to break the StructureMap.config file into smaller pieces.
    /// </summary>
    [Pluggable("XmlFile", "")]
    public class XmlFileMementoSource : XmlMementoSource
    {
        private string _filePath;
        private string _xpath;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="FilePath">Path to the xml file that contains the instance configuration</param>
        /// <param name="XPath">XPath expression to the parent node that contains the InstanceMemento nodes.
        /// If empty, it defaults to the top node</param>
        /// <param name="NodeName">The name of the nodes that are InstanceMemento nodes.  Useful to store 
        /// different types of instances in the same file</param>
        [DefaultConstructor]
        public XmlFileMementoSource(string FilePath, string XPath, string NodeName)
            : base(NodeName, "Type", "Key", XmlMementoStyle.NodeNormalized)
        {
            _filePath = FilePath;
            _xpath = XPath;
        }

        public XmlFileMementoSource(string FilePath, string XPath, string NodeName, XmlMementoStyle style)
            : base(NodeName, "Type", "Key", style)
        {
            _filePath = FilePath;
            _xpath = XPath;
        }

        public string FilePath
        {
            get { return _filePath; }
        }

        public string XPath
        {
            get { return _xpath; }
        }


        protected override XmlNode getRootNode()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(getFilePath());

            XmlNode node = null;
            if (_xpath == null || _xpath == string.Empty)
            {
                node = doc.DocumentElement;
            }
            else
            {
                node = doc.DocumentElement.SelectSingleNode(_xpath);
            }

            return node;
        }

        private string getFilePath()
        {
            // If it is a relative path, assume the file is in the same directory as the configuration file
            // crude work-around for web application problems
            if (!Path.IsPathRooted(_filePath))
            {
                string relativePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                relativePath += "\\" + _filePath;
                relativePath = relativePath.Replace("\\\\", "\\");

                return relativePath;
            }
            else
            {
                return _filePath;
            }
        }


        public override string Description
        {
            get
            {
                string msg = "XmlFileMementoSource:  " + _filePath;
                return msg;
            }
        }
    }
}