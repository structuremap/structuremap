using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration.Xml;

namespace StructureMap.Xml.Testing
{
    [TestFixture]
    public class XmlAttributeInstanceMementoTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            string xml = "<Instance PluggedType=\"Color\" Key=\"Red\" color=\"red\"/>";
            _memento = buildMemento(xml);
        }

        #endregion

        private XmlAttributeInstanceMemento _memento;


        private XmlAttributeInstanceMemento buildMemento(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            return new XmlAttributeInstanceMemento(doc.DocumentElement);
        }

        [Test]
        public void GetChildIsNotNull()
        {
            XmlElement element = _memento.InnerElement;
            XmlElement childElement = element.OwnerDocument.CreateElement("rule");
            element.AppendChild(childElement);
            childElement.SetAttribute("PluggedType", "theConcreteKey");
            childElement.SetAttribute("Key", "theInstanceKey");
            childElement.SetAttribute("prop1", "thePropertyValue");

            InstanceMemento memento = _memento.GetChildMemento("rule");
            Assert.IsNotNull(memento);
            Assert.AreEqual("theInstanceKey", memento.InstanceKey);
            Assert.AreEqual("thePropertyValue", memento.GetProperty("prop1"));
            Assert.IsFalse(memento.IsDefault);
            Assert.IsFalse(memento.IsReference);
        }

        [Test]
        public void GetChildrenArray()
        {
            XmlElement element = _memento.InnerElement;

            XmlElement rulesElement = element.OwnerDocument.CreateElement("rules");
            element.AppendChild(rulesElement);

            XmlElement childElement1 = element.OwnerDocument.CreateElement("rule");
            rulesElement.AppendChild(childElement1);
            childElement1.SetAttribute("PluggedType", "theConcreteKey");
            childElement1.SetAttribute("Key", "theInstanceKey");
            childElement1.SetAttribute("prop1", "thePropertyValue");

            var childElement2 = (XmlElement) childElement1.CloneNode(true);
            childElement2.SetAttribute("prop1", "different");
            rulesElement.AppendChild(childElement2);

            InstanceMemento[] mementoArray = _memento.GetChildrenArray("rules");
            Assert.AreEqual(2, mementoArray.Length);

            Assert.AreEqual("different", mementoArray[1].GetProperty("prop1"));
        }

        [Test]
        public void GetChildReturnsNullIfItDoesNotExist()
        {
            InstanceMemento memento = _memento.GetChildMemento("rule");
            Assert.IsNull(memento);
        }

        [Test]
        public void GetProperty()
        {
            Assert.AreEqual("red", _memento.GetProperty("color"));
        }

        [Test]
        public void IfNoChildrenReturnNull()
        {
            InstanceMemento[] mementoArray = _memento.GetChildrenArray("rules");
            Assert.IsNull(mementoArray);
        }

        [Test]
        public void InstanceKey()
        {
            Assert.AreEqual("Red", _memento.InstanceKey);
        }

        [Test]
        public void IsDefaultIsFalse()
        {
            Assert.IsFalse(_memento.IsDefault);
        }

        [Test]
        public void IsReferenceIsFalse()
        {
            Assert.IsFalse(_memento.IsReference);
        }

        [Test]
        public void ReferencedMemento()
        {
            _memento.InnerElement.SetAttribute("PluggedType", string.Empty);

            Assert.IsTrue(_memento.IsReference);
            Assert.IsFalse(_memento.IsDefault);
            Assert.AreEqual("Red", _memento.ReferenceKey);
        }
    }
}