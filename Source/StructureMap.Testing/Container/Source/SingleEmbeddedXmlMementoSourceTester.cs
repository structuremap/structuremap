using System.Reflection;
using NUnit.Framework;
using StructureMap.Source;

namespace StructureMap.Testing.Container.Source
{
    [TestFixture]
    public class SingleEmbeddedXmlMementoSourceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            string pathName = GetType().FullName.Replace(GetType().Name, "EmbeddedMementoFile.xml");
            _source =
                new SingleEmbeddedXmlMementoSource("Instance", XmlMementoStyle.AttributeNormalized,
                                                   Assembly.GetExecutingAssembly(), pathName);
        }

        #endregion

        private SingleEmbeddedXmlMementoSource _source;

/*
	<Instance Type="Type1" Key="Instance1" color="red" state="Texas"/>
	<Instance Type="Type2" Key="Instance2" color="green" state="Missouri"/>
 */


        [Test]
        public void FetchAllMementos()
        {
            Assert.AreEqual(2, _source.GetAllMementos().Length);
        }

        [Test]
        public void FetchOneMemento()
        {
            InstanceMemento memento = _source.GetMemento("Instance1");
            Assert.AreEqual("red", memento.GetProperty("color"));
            Assert.AreEqual("Instance1", memento.InstanceKey);
        }
    }
}