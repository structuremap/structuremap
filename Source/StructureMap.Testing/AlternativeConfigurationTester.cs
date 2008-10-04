using System;
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

            DataMother.WriteDocument("Config1.xml");
            DataMother.WriteDocument("Config2.xml");
            DataMother.WriteDocument("FullTesting.XML");
        }

        [TearDown]
        public void TearDown()
        {
            DataMother.RestoreStructureMapConfig();
        }

        #endregion

        public void assertTheDefault(string color, Action<ConfigurationExpression> action)
        {
            var container = new Container(action);

            container.GetInstance<IWidget>().ShouldBeOfType<ColorWidget>().Color.ShouldEqual(color);
        }

        [Test]
        public void AddNodeDirectly()
        {
            string xml = "<StructureMap><Assembly Name=\"StructureMap.Testing.GenericWidgets\"/></StructureMap>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);


            var container = new Container(x =>
            {
                x.AddConfigurationFromXmlFile("StructureMap.config");
                x.AddConfigurationFromNode(doc.DocumentElement);
            });

            container.GetInstance<IPlug<string>>().ShouldNotBeNull();
        }

        [Test]
        public void NotTheDefault()
        {
            assertTheDefault("Orange", x =>
            {
                x.AddConfigurationFromXmlFile("Config1.xml");
            });
        }

        [Test]
        public void WithTheDefault()
        {
            ObjectFactory.Initialize(x =>
            {
                x.UseDefaultStructureMapConfigFile = true;
            });

            ObjectFactory.GetInstance<IWidget>().ShouldBeOfType<ColorWidget>().Color.ShouldEqual("Red");
        }
    }
}