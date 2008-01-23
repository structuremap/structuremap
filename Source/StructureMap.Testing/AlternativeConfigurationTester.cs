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
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            DataMother.BackupStructureMapConfig();

            ObjectFactory.ReInitialize();
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


            DataMother.RestoreStructureMapConfig();
        }

        #endregion

        public void assertTheDefault(string color)
        {
            ColorWidget widget = (ColorWidget) ObjectFactory.GetInstance<IWidget>();
            Assert.AreEqual(color, widget.Color);
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

        [Test]
        public void JustToMakeSureTheSecondFamilyDoesNotOverride()
        {
            StructureMapConfiguration.UseDefaultStructureMapConfigFile = true;
            StructureMapConfiguration.IncludeConfigurationFromFile("Config1.xml");
            ObjectFactory.Reset();

            assertTheDefault("Red");
        }

        [Test]
        public void NotTheDefault()
        {
            try
            {
                StructureMapConfiguration.UseDefaultStructureMapConfigFile = false;
                StructureMapConfiguration.IgnoreStructureMapConfig = true;
                StructureMapConfiguration.IncludeConfigurationFromFile("Config1.xml");
                ObjectFactory.Reset();

                assertTheDefault("Orange");
            }
            finally
            {
                DataMother.RestoreStructureMapConfig();
            }
        }

        [Test]
        public void WithTheDefault()
        {
            assertTheDefault("Red");
        }
    }
}