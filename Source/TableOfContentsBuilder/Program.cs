using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using WatiN.Core;

namespace TableOfContentsBuilder
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            FileExtensions.BaseUrl = args.Length == 0 ? string.Empty : args[0];
            FileExtensions.Directory = @"file:///c:\code\structuremap\source\Html";
            var reader = new MenuReader();

            var filename = Path.Combine(@"c:\code\structuremap\source\Html", "ToC-Bullet-List.xml");
            reader.Write().Save(filename);

            Process.Start(filename);
        }
    }

    public class MenuReader
    {
        

        public XmlDocument Write()
        {
            IE.Settings.MakeNewIeInstanceVisible = false;

            var browser = new IE("menu.htm".File());
            var writer = new XmlWriter();

            foreach (Link link in browser.Links)
            {
                if (link.Url.StartsWith("file:///") && !link.Url.Contains("TableOfContents.htm"))
                {
                    processFile(link, writer);
                }
            }

            browser.Close();

            return writer.XmlDocument;
        }

        private void processFile(Link link, XmlWriter writer)
        {
            string path = link.Url;
            string title = link.InnerHtml;

            writer.WritePage(title, path);

            Debug.WriteLine(title + " = " + path);

            
            var browser = new IE(path, false);
            foreach (Element header in browser.Elements)
            {
                if (header.TagName == "H2")
                {
                    processHeader(header, writer, path);
                }   

                if (header.TagName == "H4")
                {
                    processSubHeader(header, writer, path);
                }
            }
            browser.Close();
        }

        private void processSubHeader(Element header, XmlWriter writer, string path)
        {
            string text = header.InnerHtml;
            writer.WriteSubHeader(text, path);


            Debug.WriteLine("h4 --> " + header.InnerHtml);
        }

        private void processHeader(Element header, XmlWriter writer, string path)
        {
            string text = header.InnerHtml;
            writer.WriteMainHeader(text, path);


            Debug.WriteLine("h2 --> " + header.InnerHtml);
        }
    }

    public class XmlWriter
    {
        private readonly XmlDocument _doc = new XmlDocument();
        private XmlElement _root;
        private XmlElement _pageContainer;
        private XmlElement _currentSection;
        private int _section = 0;

        public XmlWriter()
        {
            _root = _doc.WithRoot("div").AddElement("ul");
        }

        public XmlDocument XmlDocument
        {
            get { return _doc; }
        }

        public void WritePage(string title, string path)
        {
            _section = 0;
            _pageContainer = _root.AddElement("li", x =>
            {
                x.WriteLink(title, path);
            }).AddElement("ul");
        }


        public void WriteMainHeader(string text, string path)
        {
            _currentSection = _pageContainer.AddElement("li", x =>
            {
                x.WriteLink(text, path + "#section" + _section.ToString());
                _section++;
            }).AddElement("ul");
        }

        public void WriteSubHeader(string text, string path)
        {
            _currentSection.AddElement("li").WriteLink(text, path + "#section" + _section);
            _section++;
        }
    }

    public static class FileExtensions
    {
        public static string BaseUrl { get; set; }
        public static string Directory { get; set; }

        public static string File(this string file)
        {
            return Path.Combine(Directory, file);
        }

        public static string Url(this string url)
        {
            string[] parts = url.Split('/');
            string name = parts[parts.Length - 1];
            return Path.Combine(BaseUrl, name).Replace('\\', '/');
        }

        public static void WriteLink(this XmlElement element, string title, string url)
        {
            element.AddElement("a").WithAtt("href", url.Url()).WithInnerText(title);
        }
    }

 
}