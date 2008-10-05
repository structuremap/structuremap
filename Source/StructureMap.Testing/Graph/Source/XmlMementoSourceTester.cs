using NUnit.Framework;
using StructureMap.Source;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing.Graph.Source
{
    [TestFixture]
    public class XmlMementoSourceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var theFileName = "RuleSource.xml";
            DataMother.WriteDocument(theFileName);
            source = new XmlFileMementoSource(theFileName, "", "Rule");
        }

        #endregion

        private MementoSource source;


        [Test]
        public void GetAll()
        {
            // There are 4 Rules defined in the XML file
            InstanceMemento[] mementos = source.GetAllMementos();

            Assert.IsNotNull(mementos);
            Assert.AreEqual(4, mementos.Length, "4 mementos from 4 XML nodes");

            foreach (InstanceMemento memento in mementos)
            {
                Assert.IsNotNull(memento);
            }
        }

        [Test]
        public void GetComplex1()
        {
            InstanceMemento memento = source.GetMemento("Complex1");

            Assert.IsNotNull(memento);


            Assert.AreEqual("Complex1", memento.InstanceKey, "InstanceKey");
            Assert.AreEqual("Complex", memento.ConcreteKey, "ConcreteTypeKey");
            Assert.AreEqual("Red", memento.GetProperty("String"), "String");
            Assert.AreEqual("Green", memento.GetProperty("String2"), "String2");
            Assert.AreEqual("1", memento.GetProperty("Int"), "Int");
            Assert.AreEqual("2", memento.GetProperty("Long"), "Long");
            Assert.AreEqual("3", memento.GetProperty("Byte"), "Byte");
            Assert.AreEqual("4.5", memento.GetProperty("Double"), "Double");
            Assert.AreEqual("true", memento.GetProperty("Bool"), "Bool");
        }
    }
}