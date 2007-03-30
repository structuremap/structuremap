using System.Xml;
using NUnit.Framework;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing
{
    [TestFixture]
    public class AlternativeConfigurationTester
    {
        [SetUp]
        public void SetUp()
        {
            StructureMapConfiguration.ResetAll();
            DataMother.WriteDocument("Config1.xml");
            DataMother.WriteDocument("Config2.xml");
            DataMother.WriteDocument("FullTesting.XML");
        }

        [TearDown]
        public void TearDown()
        {
            StructureMapConfiguration.ResetAll();
            ObjectFactory.Reset();
        }


        public void assertTheDefault(string color)
        {
            ColorWidget widget = (ColorWidget) ObjectFactory.GetInstance<IWidget>();
            Assert.AreEqual(color, widget.Color);
        }

        [Test]
        public void WithTheDefault()
        {
            assertTheDefault("Red");
        }

        [Test]
        public void NotTheDefault()
        {
            StructureMapConfiguration.UseDefaultStructureMapConfigFile = false;
            StructureMapConfiguration.IncludeConfigurationFromFile("Config1.xml");
            ObjectFactory.Reset();

            assertTheDefault("Orange");
        }

        [Test]
        public void JustToMakeSureTheSecondFamilyDoesNotOverride()
        {
            StructureMapConfiguration.UseDefaultStructureMapConfigFile = true;
            StructureMapConfiguration.IncludeConfigurationFromFile("Config1.xml");
            ObjectFactory.Reset();

            assertTheDefault("Red");
        }

        [Test]
        public void AddNodeDirectly()
        {
            string xml = "<StructureMap><Assembly Name=\"StructureMap.Testing.GenericWidgets\"/></StructureMap>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);


            StructureMapConfiguration.UseDefaultStructureMapConfigFile = true;
            StructureMapConfiguration.IncludeConfigurationFromNode(doc.DocumentElement);
            ObjectFactory.Reset();

            IPlug<string> service = ObjectFactory.GetInstance<IPlug<string>>();
            Assert.IsNotNull(service);
        }
    }
}