using System.Xml;
using NUnit.Framework;
using StructureMap.Source;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing.Container.Source
{
    [TestFixture]
    public class XmlInstanceMementoTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            XmlDocument doc = DataMother.GetXmlDocument("XmlInstanceMemento.xml");

            memento = new XmlNodeInstanceMemento(doc.DocumentElement, "Type", "Key");
        }

        #endregion

        private XmlNodeInstanceMemento memento;

        public XmlInstanceMementoTester()
        {
        }


        [Test]
        public void GetChildMemento()
        {
            InstanceMemento child = memento.GetChildMemento("Child1");
            Assert.IsNotNull(child);

            Assert.AreEqual("Child1", child.ConcreteKey, "Type is Child1");
            Assert.AreEqual("D", child.GetProperty("Prop4"), "Prop4");
        }

        [Test]
        public void GetLargeStringInAttributeMemento()
        {
            XmlDocument document = DataMother.GetXmlDocument("CDataTest.xml");
            XmlNode node = document.DocumentElement.LastChild;

            XmlAttributeInstanceMemento memento = new XmlAttributeInstanceMemento(node);
            Assert.AreEqual("select * from table", memento.GetProperty("bigProp"));
        }

        [Test]
        public void GetLargeStringInNodeMemento()
        {
            XmlDocument document = DataMother.GetXmlDocument("CDataTest.xml");
            XmlNode node = document.DocumentElement.FirstChild;

            XmlNodeInstanceMemento memento = new XmlNodeInstanceMemento(node, "Type", "Key");
            Assert.AreEqual("select * from table", memento.GetProperty("bigProp"));
        }
    }
}