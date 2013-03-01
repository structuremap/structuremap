using System.Linq;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing;
using StructureMap.Testing.Widget;
using StructureMap.Xml.Testing.TestData;

namespace StructureMap.Xml.Testing
{
    [TestFixture]
    public class ShortcuttedInstanceNodeTester
    {
        [SetUp]
        public void SetUp()
        {
            _graph = DataMother.GetPluginGraph("ShortInstance.xml");
            theContainer = new Container(_graph);
        }

        private Container theContainer;
        private PluginGraph _graph;


        [Test]
        public void GetAllRules()
        {
            theContainer.GetAllInstances<Rule>()
                .Count().ShouldEqual(1);
        }

        [Test]
        public void GetTheRule()
        {
            theContainer.GetInstance<Rule>("Blue")
                        .ShouldBeOfType<ColorRule>()
                        .Color.ShouldEqual("Blue");
        }

        [Test]
        public void GetTheWidget()
        {
            var widget = (ColorWidget) theContainer.GetInstance<IWidget>("Red");
            Assert.AreEqual("Red", widget.Color);

            var widget2 = (ColorWidget) theContainer.GetInstance<IWidget>("Red");
            Assert.AreNotSame(widget, widget2);
        }

        [Test]
        public void GetUnKeyedInstancesToo()
        {
            theContainer.GetAllInstances<IWidget>()
                    .Count().ShouldEqual(4);
        }
    }
}