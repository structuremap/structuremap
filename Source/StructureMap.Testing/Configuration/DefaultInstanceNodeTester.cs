using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Configuration
{
    [TestFixture]
    public class DefaultInstanceNodeTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            string xml = @"
<StructureMap MementoStyle='Attribute'>
  <DefaultInstance PluginType='StructureMap.Testing.Widget.IWidget,StructureMap.Testing.Widget' PluggedType='StructureMap.Testing.Widget.ColorWidget,StructureMap.Testing.Widget' Color='Red' />  
  <DefaultInstance PluginType='StructureMap.Testing.Widget.Rule,StructureMap.Testing.Widget' PluggedType='StructureMap.Testing.Widget.ColorRule,StructureMap.Testing.Widget' Color='Blue' Scope='Singleton' Key='TheBlueOne'/>  
</StructureMap>
";

            _graph = DataMother.BuildPluginGraphFromXml(xml);
            _manager = new InstanceManager(_graph);
        }

        #endregion

        private InstanceManager _manager;
        private PluginGraph _graph;

        [Test]
        public void DefaultNameOfRule()
        {
            PluginFamily family = _graph.FindFamily(typeof (Rule));
            Assert.AreEqual("TheBlueOne", family.DefaultInstanceKey);
        }

        [Test]
        public void GetTheRule()
        {
            ColorRule rule = (ColorRule) _manager.GetInstance<Rule>();
            Assert.AreEqual("Blue", rule.Color);

            ColorRule rule2 = (ColorRule) _manager.GetInstance<Rule>();
            Assert.AreSame(rule, rule2);
        }

        [Test]
        public void GetTheWidget()
        {
            ColorWidget widget = (ColorWidget) _manager.GetInstance<IWidget>();
            Assert.AreEqual("Red", widget.Color);

            ColorWidget widget2 = (ColorWidget) _manager.GetInstance<IWidget>();
            Assert.AreNotSame(widget, widget2);
        }
    }
}