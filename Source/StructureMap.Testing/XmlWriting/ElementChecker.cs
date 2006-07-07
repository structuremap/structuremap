using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Xml;
using NUnit.Framework;

namespace StructureMap.Testing.XmlWriting
{
	public class ElementChecker
	{
		private readonly string _nodeName;
		private NameValueCollection _attributes;
		private ArrayList _childNodes;

		public ElementChecker(string nodeName)
		{
			_nodeName = nodeName;
			_attributes = new NameValueCollection();
			_childNodes = new ArrayList();
		}


		public ElementChecker(XmlElement element)
			: this(element.Name)
		{
			foreach (XmlAttribute att in element.Attributes)
			{
				// ignore empty attributes.  Assume that this is an optional attribute
				if (att.Value != string.Empty)
				{
					_attributes[att.Name] = att.Value;
				}
			}

			foreach (XmlNode node in element.ChildNodes)
			{
				XmlElement elemChild = node as XmlElement;
				if (elemChild != null)
				{
					_childNodes.Add(new ElementChecker(elemChild));
				}
			}
		}

		public string this[string attName]
		{
			get { return _attributes[attName]; }
			set { _attributes[attName] = value; }
		}

		public void AddChildNode(ElementChecker checker)
		{
			_childNodes.Add(checker);
		}

		public void Check(XmlElement element)
		{
			Assert.IsNotNull(element);

			try
			{
				Assert.AreEqual(_nodeName, element.Name, "Element Name");
				Assert.AreEqual(_attributes.Count, getAttributeCount(element), "Attribute Count for " + _nodeName);

				foreach (XmlAttribute att in element.Attributes)
				{
					string expected = this[att.Name];
					if (expected == null)
					{
						expected = string.Empty;
					}

					Assert.AreEqual(expected, att.Value, att.Name);
				}

				Assert.AreEqual(_childNodes.Count, element.ChildNodes.Count, "Wrong number of child nodes for " + _nodeName);
				for (int i = 0; i < _childNodes.Count; i++)
				{
					ElementChecker checker = (ElementChecker) _childNodes[i];
					XmlElement elemChild = (XmlElement) element.ChildNodes[i];

					checker.Check(elemChild);
				}
			}
			catch (Exception)
			{
				Console.WriteLine(element.OuterXml);
				Debug.WriteLine(element.OuterXml);
				throw;
			}


		}

		private static int getAttributeCount(XmlElement element)
		{
			int returnValue = 0;

			foreach (XmlAttribute att in element.Attributes)
			{
				if (att.Value != string.Empty)
				{
					returnValue++;
				}
			}

			return returnValue;
		}


		public static void AssertXmlElement(string xpath, XmlElement expectedRoot, XmlElement actualRoot)
		{
			XmlElement elemExpected = (XmlElement) expectedRoot.SelectSingleNode(xpath);
			XmlElement elemActual = (XmlElement) actualRoot.SelectSingleNode(xpath);
			ElementChecker checker = new ElementChecker(elemExpected);
			checker.Check(elemActual);
		}

	}
}