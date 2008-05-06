using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;
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
            _manager = new InstanceManager(_graph);
        }

        #endregion

        private InstanceManager _manager;
        private PluginGraph _graph;

        [Test]
        public void CreateTheInferredPluginCorrectly()
        {
            // Who needs the Law of Demeter?
            _graph.Seal();

            Instance[] instances = _graph.FindFamily(typeof (IWidget)).GetAllInstances();
            foreach (Instance instance in instances)
            {
                Debug.WriteLine(instance.Name + ", " + instance.GetType().FullName);
            }
            Assert.AreEqual(4, instances.Length);
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
            ColorRule rule = (ColorRule) _manager.CreateInstance<Rule>("Blue");
            Assert.AreEqual("Blue", rule.Color);
        }

        [Test]
        public void GetTheWidget()
        {
            ColorWidget widget = (ColorWidget) _manager.CreateInstance<IWidget>("Red");
            Assert.AreEqual("Red", widget.Color);

            ColorWidget widget2 = (ColorWidget) _manager.CreateInstance<IWidget>("Red");
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