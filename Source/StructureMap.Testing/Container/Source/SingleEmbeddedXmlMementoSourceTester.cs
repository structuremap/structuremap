using System.Reflection;
using NUnit.Framework;
using StructureMap.Source;

namespace StructureMap.Testing.Container.Source
{
	[TestFixture]
	public class SingleEmbeddedXmlMementoSourceTester
	{
		private SingleEmbeddedXmlMementoSource _source;


		[SetUp]
		public void SetUp()
		{
			string pathName = this.GetType().FullName.Replace(this.GetType().Name, "EmbeddedMementoFile.xml");
			_source = new SingleEmbeddedXmlMementoSource("Instance", XmlMementoStyle.AttributeNormalized, Assembly.GetExecutingAssembly(), pathName);
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
