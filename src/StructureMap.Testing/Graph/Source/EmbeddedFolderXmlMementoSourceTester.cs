using System.Reflection;
using NUnit.Framework;
using StructureMap.Source;

namespace StructureMap.Testing.Graph.Source
{
    [TestFixture]
    public class EmbeddedFolderXmlMementoSourceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            string folderPath = GetType().FullName.Replace(GetType().Name, "Mementos");
            string assemblyName = Assembly.GetExecutingAssembly().FullName;
            _source =
                new EmbeddedFolderXmlMementoSource(XmlMementoStyle.AttributeNormalized, assemblyName, folderPath, "xml");
        }

        #endregion

        private EmbeddedFolderXmlMementoSource _source;

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