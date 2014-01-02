using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.Widget;
using StructureMap.Xml.Testing.TestData;

namespace StructureMap.Xml.Testing
{
    [TestFixture, Ignore("Until we rewrite the Xml config")]
    public class DefaultInstanceNodeTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            string xml =
                @"
<StructureMap>
  <DefaultInstance PluginType='StructureMap.Testing.Widget.IWidget,StructureMap.Testing.Widget' PluggedType='StructureMap.Testing.Widget.ColorWidget,StructureMap.Testing.Widget' color='Red' />  
  <DefaultInstance PluginType='StructureMap.Testing.Widget.Rule,StructureMap.Testing.Widget' PluggedType='StructureMap.Testing.Widget.ColorRule,StructureMap.Testing.Widget' color='Blue' Scope='Singleton' Key='TheBlueOne'/>  
</StructureMap>
";

            _graph = DataMother.BuildPluginGraphFromXml(xml);
            _manager = new Container(_graph);
        }

        #endregion

        private Container _manager;
        private PluginGraph _graph;

        [Test]
        public void DefaultNameOfRule()
        {
            var family = _graph.Families[typeof(Rule)];
            Assert.AreEqual("TheBlueOne", family.GetDefaultInstance().Name);
        }

        [Test, Ignore("Until GH-91 is fixed")]
        public void GetTheRule()
        {
            var rule = (ColorRule) _manager.GetInstance<Rule>();
            Assert.AreEqual("Blue", rule.Color);

            var rule2 = (ColorRule) _manager.GetInstance<Rule>();
            Assert.AreSame(rule, rule2);
        }

        [Test]
        public void GetTheWidget()
        {
            var widget = (ColorWidget) _manager.GetInstance<IWidget>();
            Assert.AreEqual("Red", widget.Color);

            var widget2 = (ColorWidget) _manager.GetInstance<IWidget>();
            Assert.AreNotSame(widget, widget2);
        }
    }
}