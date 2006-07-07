using System.Reflection;
using NUnit.Framework;
using StructureMap.Source;

namespace StructureMap.Testing.Container.Source
{
	[TestFixture]
	public class EmbeddedFolderXmlMementoSourceTester
	{
		private EmbeddedFolderXmlMementoSource _source;

		[SetUp]
		public void SetUp()
		{
			string folderPath = this.GetType().FullName.Replace(this.GetType().Name, "Mementos");
			string assemblyName = Assembly.GetExecutingAssembly().FullName;
			_source = new EmbeddedFolderXmlMementoSource(XmlMementoStyle.AttributeNormalized, assemblyName, folderPath, "xml");
		}
		/*
			<Instance Type="Type1" Key="Instance1" color="red" state="Texas"/>
			<Instance Type="Type2" Key="Instance2" color="green" state="Missouri"/>
		 */


		[Test]
		public void FetchOneMemento()
		{
			InstanceMemento memento = _source.GetMemento("Instance1");
			Assert.AreEqual("red", memento.GetProperty("color"));
			Assert.AreEqual("Instance1", memento.InstanceKey);
		}

		[Test]
		public void FetchAllMementos()
		{
			Assert.AreEqual(2, _source.GetAllMementos().Length);
		}
	}
}
