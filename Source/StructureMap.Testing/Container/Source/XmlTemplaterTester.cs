using System.Collections;
using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration.Mementos;
using StructureMap.Source;
using StructureMap.Testing.TestData;
using StructureMap.Testing.XmlWriting;

namespace StructureMap.Testing.Container.Source
{
    [TestFixture]
    public class XmlTemplaterTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _document = DataMother.GetXmlDocument("Templater.xml");
        }

        #endregion

        private XmlDocument _document;

        [Test]
        public void GetSubstititionsExplicitly()
        {
            XmlNode node = _document.DocumentElement.FirstChild;
            XmlElement element = (XmlElement) node;
            element.SetAttribute(InstanceMemento.SUBSTITUTIONS_ATTRIBUTE, "direction,color");

            XmlTemplater templater = new XmlTemplater(node);
            string[] substitutions = templater.Substitutions;

            Assert.AreEqual(2, substitutions.Length);
            ArrayList list = new ArrayList(substitutions);

            Assert.IsTrue(list.Contains("color"));
            Assert.IsTrue(list.Contains("direction"));
        }

        [Test]
        public void GetSubstitutionsImplicitly()
        {
            XmlNode node = _document.DocumentElement.FirstChild;
            XmlTemplater templater = new XmlTemplater(node);
            string[] substitutions = templater.Substitutions;

            Assert.AreEqual(4, substitutions.Length);
            ArrayList list = new ArrayList(substitutions);

            Assert.IsTrue(list.Contains("color"));
            Assert.IsTrue(list.Contains("name"));
            Assert.IsTrue(list.Contains("state"));
            Assert.IsTrue(list.Contains("direction"));
        }


        [Test]
        public void HandleStringEmpty()
        {
            MemoryInstanceMemento memento = new MemoryInstanceMemento("", "");
            memento.SetProperty("color", "");

            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement("top");
            doc.AppendChild(element);
            element.SetAttribute("Color", "{color}");

            XmlTemplater templater = new XmlTemplater(element);

            XmlElement result = (XmlElement) templater.SubstituteTemplates(element, memento);
            Assert.AreEqual(InstanceMemento.EMPTY_STRING, result.GetAttribute("Color"));
        }

        [Test]
        public void MakeSubstitutionsInXmlNode()
        {
            MemoryInstanceMemento memento = new MemoryInstanceMemento("", "");
            memento.SetProperty("color", "blue");
            memento.SetProperty("name", "ObiWan");
            memento.SetProperty("state", "New York");
            memento.SetProperty("direction", "North");

            XmlNode templateNode = _document.DocumentElement.FirstChild;
            XmlTemplater templater = new XmlTemplater(templateNode);

            XmlNode expectedNode = _document.DocumentElement.LastChild;
            ElementChecker checker = new ElementChecker((XmlElement) expectedNode);

            XmlElement actualElement = (XmlElement) templater.SubstituteTemplates(templateNode, memento);

            Assert.IsFalse(ReferenceEquals(templateNode, actualElement));
            checker.Check(actualElement);
        }
    }
}