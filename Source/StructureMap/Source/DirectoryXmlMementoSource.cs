using System;
using System.Collections;
using System.IO;
using System.Xml;
using StructureMap.Configuration;

namespace StructureMap.Source
{
    /// <summary>
    /// Implementation of MementoSource that stores and retrieves an XmlInstanceMemento per file in a named directory.  
    /// DirectoryXmlMementoSource is meant to simplify complicated object graph configurations by isolating each instance to a separate
    /// editable file.
    /// </summary>
    [Pluggable("DirectoryXml")]
    public class DirectoryXmlMementoSource : MementoSource
    {
        private readonly string _directory;
        private readonly string _extension;
        private XmlMementoCreator _mementoCreator;

        /// <summary>
        /// Stores an Xml InstanceMemento per file in a directory
        /// </summary>
        /// <param name="directory">A ";" delimited list of directories to look for mementos.  DirectoryXmlMementoSource 
        /// will use the FIRST directory it finds</param>
        /// <param name="extension">The file extension of the InstanceMemento files without a dot.  Typically "xml"</param>
        /// <param name="mementoStyle">NodeNormalized or AttributeNormalized</param>
        public DirectoryXmlMementoSource(string directory, string extension, XmlMementoStyle mementoStyle) : base()
        {
            _extension = extension;
            _mementoCreator =
                new XmlMementoCreator(mementoStyle, XmlConstants.TYPE_ATTRIBUTE, XmlConstants.KEY_ATTRIBUTE);

            string[] searchPaths = directory.Split(';');
            foreach (string searchPath in searchPaths)
            {
                string resolvedPath = resolvePath(searchPath);
                if (Directory.Exists(resolvedPath))
                {
                    _directory = resolvedPath;
                    break;
                }
            }


            if (_directory == null)
            {
                throw new ApplicationException("Could not find any of the SearchPaths in the directory path");
            }
        }

        public override MementoSourceType SourceType
        {
            get { return MementoSourceType.External; }
        }

        public override string Description
        {
            get
            {
                return string.Format("DirectoryXml MementoSource reading from {0} with extension {1}",
                                     Path.GetFullPath(_directory), _extension);
            }
        }

        private string resolvePath(string directory)
        {
            string returnValue = string.Empty;

            if (!Path.IsPathRooted(directory))
            {
                returnValue = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                returnValue += "\\" + directory;
                returnValue = returnValue.Replace("\\\\", "\\");
            }
            else
            {
                returnValue = Path.GetFullPath(directory);
            }

            return returnValue;
        }


        protected override InstanceMemento[] fetchInternalMementos()
        {
            string[] fileNames = Directory.GetFiles(_directory, "*." + _extension);
            ArrayList list = new ArrayList();
            foreach (string fileName in fileNames)
            {
                InstanceMemento memento = loadMementoFromFile(fileName);
                list.Add(memento);
            }

            return (InstanceMemento[]) list.ToArray(typeof (InstanceMemento));
        }


        protected internal override bool containsKey(string instanceKey)
        {
            string filePath = getFilePath(instanceKey);
            return File.Exists(filePath);
        }

        private string getFilePath(string instanceKey)
        {
            return string.Format("{0}\\{1}.{2}", _directory, instanceKey, _extension);
        }

        protected override InstanceMemento retrieveMemento(string instanceKey)
        {
            string filePath = getFilePath(instanceKey);
            return loadMementoFromFile(filePath);
        }

        private InstanceMemento loadMementoFromFile(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            doc.DocumentElement.SetAttribute(XmlConstants.KEY_ATTRIBUTE, fileName);

            return createMemento(doc);
        }

        protected virtual InstanceMemento createMemento(XmlDocument doc)
        {
            return _mementoCreator.CreateMemento(doc.DocumentElement);
        }

        [ValidationMethod]
        public void Validate()
        {
            if (!Directory.Exists(_directory))
            {
                string message = string.Format("The directory {0} could not be found", _directory);
                throw new ApplicationException(message);
            }
        }
    }
}