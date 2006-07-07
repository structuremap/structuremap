using System;
using System.Xml;

namespace StructureMap.Client.Views
{
	public class HTMLBuilder
	{
		private XmlDocument _document;

		public HTMLBuilder()
		{
			_document = new XmlDocument();
			XmlElement div = _document.CreateElement("div");
			_document.AppendChild(div);
		}

		public void AddHeader(string headerText)
		{
			XmlElement header = _document.CreateElement("h3");
			header.SetAttribute("style", "COLOR: #316592;FONT-SIZE: large;FONT-WEIGHT: 500;TEXT-DECORATION: underline");
			header.InnerText = headerText;
			_document.DocumentElement.AppendChild(header);
		}

		public TableMaker StartTable()
		{
			XmlElement div = _document.CreateElement("div");
			div.SetAttribute("class", "DispTab");
			_document.DocumentElement.AppendChild(div);
			XmlElement table = _document.CreateElement("table");
			div.AppendChild(table);

			return new TableMaker(table);
		}

		public string HTML
		{
			get
			{
				return _document.DocumentElement.OuterXml;
			}
		}

		public void AddSubHeader(string headerText)
		{
			XmlElement header = _document.CreateElement("h4");
			header.SetAttribute("style", "COLOR: #316592;FONT-FAMILY: Arial;FONT-SIZE: 12pt;FONT-WEIGHT: bold");
			header.InnerText = headerText;
			_document.DocumentElement.AppendChild(header);
		}

		public XmlElement AddTable()
		{
			XmlElement element = _document.CreateElement("table");
			_document.DocumentElement.AppendChild(element);

			return element;
		}

		public void AddDivider()
		{
			XmlElement br = _document.CreateElement("br");
			_document.DocumentElement.AppendChild(br);
		}
	}
}
