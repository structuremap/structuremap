using System;
using System.Xml;
using StructureMap.Configuration;

namespace StructureMap.Client.Views
{
    public class CellMaker
    {
        private readonly XmlElement _tdElement;

        public CellMaker(XmlElement tdElement)
        {
            _tdElement = tdElement;
        }

        public int Level
        {
            set
            {
                int padding = 20*value;
                string style = string.Format("padding-Left:{0}px", padding.ToString());
                _tdElement.SetAttribute("style", style);
            }
        }

        public void AddText(string text)
        {
            XmlElement span = _tdElement.OwnerDocument.CreateElement("span");
            _tdElement.AppendChild(span);
            try
            {
                span.InnerXml = text;
            }
            catch (Exception)
            {
                span.InnerText = text;
            }
        }

        public void AddLink(string text, string url)
        {
            XmlElement link = _tdElement.OwnerDocument.CreateElement("a");
            link.InnerText = text;
            link.SetAttribute("href", "http://" + url);
            link.SetAttribute("target", "_blank");
            _tdElement.AppendChild(link);
        }


        public void AddFormattedText(string message)
        {
            XmlElement pre = _tdElement.OwnerDocument.CreateElement("pre");
            pre.InnerText = message;
            _tdElement.AppendChild(pre);
        }

        public void MarkProblems(GraphObject node)
        {
            _tdElement.SetAttribute("color", "red");
            AddText(", ");
            string display = node.Problems.Length.ToString() + " Problems";
            string url = "ID=" + node.Id;

            AddLink(display, url);
        }

        public void SetColor(string color)
        {
            _tdElement.SetAttribute("bgColor", color);
        }
    }
}