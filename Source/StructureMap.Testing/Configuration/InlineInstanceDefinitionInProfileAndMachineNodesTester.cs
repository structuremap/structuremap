using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Configuration
{
    [TestFixture]
    public class InlineInstanceDefinitionInProfileAndMachineNodesTester
    {
        private PluginGraph graph
        {
            get
            {
                string xml =
                    @"
<StructureMap MementoStyle='Attribute'>
  <Assembly Name='StructureMap.Testing.Widget'/>

  <Profile Name='Blue'>
    <Override Type='StructureMap.Testing.Widget.IWidget,StructureMap.Testing.Widget'>
      <Instance Type='Color' color='Blue'/>
    </Override>
    <Override Type='StructureMap.Testing.Widget.Rule,StructureMap.Testing.Widget'>
      <Instance Type='Color' color='Blue'/>
    </Override>
  </Profile>

  <Profile Name='Green'>
    <Override Type='StructureMap.Testing.Widget.IWidget,StructureMap.Testing.Widget'>
      <Instance Type='Color' color='Green'/>
    </Override>
    <Override Type='StructureMap.Testing.Widget.Rule,StructureMap.Testing.Widget'>
      <Instance Type='Color' color='Green'/>
    </Override>
  </Profile>

  <Machine Name='GREEN-BOX' Profile='Green'/>

  <Machine Name='ORANGE-BOX'>
    <Override Type='StructureMap.Testing.Widget.IWidget,StructureMap.Testing.Widget'>
      <Instance Type='Color' color='Orange'/>
    </Override>
  </Machine>
</StructureMap>
";


                return DataMother.BuildPluginGraphFromXml(xml);
            }
        }

        public void TearDown()
        {
        }

        [Test]
        public void CanFindTheTwoPluginFamilies()
        {
            Assert.IsTrue(graph.PluginFamilies.Contains(typeof (IWidget)));
            Assert.IsTrue(graph.PluginFamilies.Contains(typeof (Rule)));
        }

        [Test]
        public void CanRenameInstanceMemento()
        {
            var memento = new MemoryInstanceMemento("concrete", "name");

            Assert.AreEqual("name", memento.InstanceKey);
            memento.InstanceKey = "Elvis";

            Assert.AreEqual("Elvis", memento.InstanceKey);
        }

        [Test]
        public void HasADefaultInstanceKey()
        {
            var manager = new Container(graph);

            manager.SetDefaultsToProfile("Green");

            var widget = (ColorWidget) manager.GetInstance<IWidget>();
            Assert.AreEqual("Green", widget.Color);
        }

        [Test]
        public void HasTheOverrideForProfile()
        {
            var manager = new Container(graph);
            manager.SetDefaultsToProfile("Blue");

            var rule = (ColorRule) manager.GetInstance<Rule>();
            Assert.AreEqual("Blue", rule.Color);

            var widget = (ColorWidget) manager.GetInstance<IWidget>();
            Assert.AreEqual("Blue", widget.Color);
        }

        [Test]
        public void SetTheProfile()
        {
            var manager = new Container(graph);
            manager.SetDefaultsToProfile("Green");

            var greenRule = (ColorRule) manager.GetInstance(typeof (Rule));
            Assert.AreEqual("Green", greenRule.Color);

            manager.SetDefaultsToProfile("Blue");

            var blueRule = (ColorRule) manager.GetInstance(typeof (Rule));
            Assert.AreEqual("Blue", blueRule.Color);
        }
    }
}