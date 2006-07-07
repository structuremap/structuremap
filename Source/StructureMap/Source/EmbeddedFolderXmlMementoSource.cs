using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Xml;
using StructureMap.Configuration;

namespace StructureMap.Source
{
	[Pluggable("EmbeddedXmlFolder")]
	public class EmbeddedFolderXmlMementoSource : MementoSource
	{
		private readonly string _assemblyName;
		private readonly string _folderPath;
		private readonly string _extension;

		private NameValueCollection _embeddedFiles;
		private Assembly _assembly;
		private XmlMementoCreator _creator;

		/// <summary>
		/// Implementation of MementoSource that stores and retrieves an XmlInstanceMemento per Embedded Resource file 
		/// in a named namespace.  EmbeddedFolderXmlMementoSource is meant to simplify complicated object graph configurations 
		/// by isolating each instance to a separate
		/// editable file.
		/// </summary>
		/// <param name="style">NodeNormalized or AttributeNormalized</param>
		/// <param name="assemblyName">The name of the Assembly with the embedded resources</param>
		/// <param name="folderPath">The root namespace of all of the mementos.</param>
		/// <param name="extension">The file extension of the memento files - "xml"</param>
		[DefaultConstructor]
		public EmbeddedFolderXmlMementoSource(XmlMementoStyle style, string assemblyName, string folderPath, string extension)
			: base()
		{
			_assemblyName = assemblyName;
			_folderPath = folderPath;
			_extension = extension;

			findEmbeddedFiles();

			_creator = new XmlMementoCreator(style, "Type", "Key");
		}

		private void findEmbeddedFiles()
		{
			_embeddedFiles = new NameValueCollection();
	
			int folderLength = _folderPath.Length + 1;
			_assembly = AppDomain.CurrentDomain.Load(_assemblyName);
			string[] fileNames = _assembly.GetManifestResourceNames();
			foreach (string fileName in fileNames)
			{
				if (fileName.StartsWith(_folderPath))
				{
					string smallName = fileName.Substring(folderLength);
					string[] parts = smallName.Split('.');
					if (parts.Length != 2)
					{
						continue;
					}

					if (parts[1] == _extension)
					{
						_embeddedFiles.Add(parts[0], fileName);
					}
				}
			}
		}


		public override string Description
		{
			get { return string.Format("EmbeddedFolderXml:  Assembly {0}, Folder {1}, Extension {2}", _assemblyName, _folderPath, _extension); }
		}

		protected override InstanceMemento[] fetchInternalMementos()
		{
			InstanceMemento[] returnValue = new InstanceMemento[_embeddedFiles.Count];
			int i = 0;
			foreach (string key in _embeddedFiles.AllKeys)
			{
				string fileName = _embeddedFiles[key];
				InstanceMemento memento = fetchMementoFromEmbeddedFile(fileName, key);
				returnValue[i++] = memento;
			}

			return returnValue;
		}

		protected internal override bool containsKey(string instanceKey)
		{
			return _embeddedFiles[instanceKey] != null;
		}

		protected override InstanceMemento retrieveMemento(string instanceKey)
		{
			string fileName = _embeddedFiles[instanceKey];
			InstanceMemento memento = fetchMementoFromEmbeddedFile(fileName, instanceKey);
			return memento;
		}

		private InstanceMemento fetchMementoFromEmbeddedFile(string fileName, string instanceKey)
		{
			Stream stream = _assembly.GetManifestResourceStream(fileName);
			XmlDocument document = new XmlDocument();
			document.Load(stream);

			document.DocumentElement.SetAttribute(XmlConstants.KEY_ATTRIBUTE, instanceKey);

			return _creator.CreateMemento(document.DocumentElement);
		}
	}
}