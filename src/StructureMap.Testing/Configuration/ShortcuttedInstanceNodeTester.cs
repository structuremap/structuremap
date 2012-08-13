using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Configuration
{
    [TestFixture]
    public class ShortcuttedInstanceNodeTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _graph = DataMother.GetPluginGraph("ShortInstance.xml");
            _manager = new Container(_graph);
        }

        #endregion

        private Container _manager;
        private PluginGraph _graph;

        [Test]
        public void CreateTheInferredPluginCorrectly()
        {
            // Who needs the Law of Demeter?
            _graph.Seal();

            PluginFamily family = _graph.FindFamily(typeof (IWidget));

            Assert.AreEqual(4, family.InstanceCount);
        }

        [Test]
        public void GetAllRules()
        {
            IList<Rule> list = _manager.GetAllInstances<Rule>();
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void GetTheRule()
        {
            var rule = (ColorRule) _manager.GetInstance<Rule>("Blue");
            Assert.AreEqual("Blue", rule.Color);
        }

        [Test]
        public void GetTheWidget()
        {
            var widget = (ColorWidget) _manager.GetInstance<IWidget>("Red");
            Assert.AreEqual("Red", widget.Color);

            var widget2 = (ColorWidget) _manager.GetInstance<IWidget>("Red");
            Assert.AreNotSame(widget, widget2);
        }

        [Test]
        public void GetUnKeyedInstancesToo()
        {
            IList<IWidget> list = _manager.GetAllInstances<IWidget>();
            Assert.AreEqual(4, list.Count);
        }
    }
}