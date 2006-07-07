using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;

namespace StructureMap.Testing.TestData
{
	public class DataMother
	{
		private static ArrayList _files = new ArrayList();

		private DataMother()
		{
		}

		public static XmlDocument GetXmlDocument(string fileName)
		{
			XmlDocument document = new XmlDocument();

			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(new DataMother().GetType(), fileName);
			document.Load(stream);

			return document;
		}

		public static void WriteDocument(string fileName)
		{
			XmlDocument document = GetXmlDocument(fileName);
			document.Save(fileName);

			_files.Add(fileName);
		}

		public static void CleanUp()
		{
			foreach (string fileName in _files)
			{
				try
				{
					File.Delete(fileName);
				}
				catch (Exception){}
			}
		}
	}
}
