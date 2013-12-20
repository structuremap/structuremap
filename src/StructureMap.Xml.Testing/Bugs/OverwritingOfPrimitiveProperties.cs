using NUnit.Framework;
using StructureMap.Configuration.Xml;
using StructureMap.Xml.Testing.TestData;

namespace StructureMap.Xml.Testing.Bugs
{
    [TestFixture]
    public class OverwritingOfPrimitiveProperties
    {
        public const string XML_FILENAME = "BUG_OverwritingOfPrimitiveProperties.xml";

        [SetUp]
        public void SetUp()
        {
            DataMother.WriteDocument(XML_FILENAME);
        }

        [Test]
        public void Test()
        {   
            ObjectFactory.Initialize(x =>
            {
                x.AddConfigurationFromXmlFile(XML_FILENAME);
            });

            IFooWithPrimitives foo = ObjectFactory.GetInstance<IFooWithPrimitives>();
            Assert.IsNotNull(foo);
            Assert.AreEqual("Test123", foo.TestValue);
            Assert.IsTrue(foo.IsTest);
        }
    }
}