using System;
using System.Xml;

namespace StructureMap.Client.Views
{
    public class TableMaker
    {
        private readonly XmlElement _tableElement;
        private string _alternatingColor = "E8E8E8";
        private XmlElement _lastRow;
        private XmlDocument _ownerDocument;
        private bool _useAlternatingColor = false;

        public TableMaker(XmlElement tableElement)
        {
            tableElement.SetAttribute("style", "border:navy groove;font-size:12pt");

            _tableElement = tableElement;
            _ownerDocument = tableElement.OwnerDocument;
        }

        public XmlElement AddRow(bool hasErrors)
        {
            _lastRow = _ownerDocument.CreateElement("tr");
            _tableElement.AppendChild(_lastRow);

            if (hasErrors)
            {
                _lastRow.SetAttribute("style", "color:red");
            }

            AlternateColors();

            return _lastRow;
        }

        public void AlternateColors()
        {
            if (_useAlternatingColor)
            {
                _lastRow.SetAttribute("bgColor", _alternatingColor);
            }
            _useAlternatingColor = !_useAlternatingColor;
        }

        public void AddHeader(string headerText)
        {
            XmlElement th = _ownerDocument.CreateElement("th");
            th.SetAttribute("style",
                            "color:white;BACKGROUND-COLOR: #316592;FONT-FAMILY: Arial;font-weight:bold;padding-Right:5px;padding-Left:5px;font-size:10pt");
            th.InnerText = headerText;
            _lastRow.AppendChild(th);
        }

        public void AddHeader(string headerText, int percentWidth)
        {
            XmlElement th = _ownerDocument.CreateElement("th");
            th.SetAttribute("style",
                            "color:white;BACKGROUND-COLOR: #316592;FONT-FAMILY: Arial;font-weight:bold;padding-Right:5px;padding-Left:5px;font-size:10pt");
            th.InnerText = headerText;
            _lastRow.AppendChild(th);

            th.SetAttribute("width", percentWidth.ToString() + "%");
        }

        private XmlElement addTableCell()
        {
            XmlElement td = _ownerDocument.CreateElement("td");
            td.SetAttribute("style", "padding-Left:5px, padding-Right:5px");

            _lastRow.AppendChild(td);
            td.SetAttribute("valign", "top");

            return td;
        }

        public void AddCell(string text)
        {
            XmlElement cell = addTableCell();
            cell.InnerText = text;
        }

        public void AddLink(string text, string href)
        {
            XmlElement cell = addTableCell();
            XmlElement link = _ownerDocument.CreateElement("a");
            link.InnerText = text;
            link.SetAttribute("href", "http://" + href);
            link.SetAttribute("target", "_blank");
            cell.AppendChild(link);
        }

        public void AddReferenceLink(string text, Guid id)
        {
            AddLink(text, "ID=" + id.ToString());
        }

        public CellMaker CreateCell()
        {
            XmlElement cell = addTableCell();
            return new CellMaker(cell);
        }

        public void AddCenteredCell(string text)
        {
            XmlElement cell = addTableCell();
            cell.SetAttribute("align", "center");
            cell.InnerText = text;
        }
    }
}