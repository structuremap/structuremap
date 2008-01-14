using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Configuration.Mementos;
using StructureMap.Graph;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Configuration
{
    [TestFixture]
    public class InlineInstanceDefinitionInProfileAndMachineNodesTester
    {
        private PluginGraph _graph;

        [SetUp]
        public void SetUp()
        {
            _graph = DataMother.GetPluginGraph("InlineInstanceInProfileAndMachine.xml");
        }

        [Test]
        public void CanRenameInstanceMemento()
        {
            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "name");

            Assert.AreEqual("name", memento.InstanceKey);
            memento.InstanceKey = "Elvis";

            Assert.AreEqual("Elvis", memento.InstanceKey);
        }


        [Test]
        public void CanFindTheTwoPluginFamilies()
        {
            Assert.IsTrue(_graph.PluginFamilies.Contains(typeof (IWidget)));
            Assert.IsTrue(_graph.PluginFamilies.Contains(typeof (Rule)));
        }

        [Test]
        public void HasTheOverrideForProfile()
        {
            Profile blueProfile = _graph.DefaultManager.GetProfile("Blue");
            Assert.IsTrue(blueProfile.HasOverride(typeof (Rule).FullName));
            Assert.IsTrue(blueProfile.HasOverride(typeof (IWidget).FullName));

            Assert.IsNotEmpty(blueProfile[typeof (Rule).FullName]);
            Assert.IsNotEmpty(blueProfile[typeof (IWidget).FullName]);

            Debug.WriteLine(blueProfile[typeof (IWidget).FullName]);

            Profile defaults = _graph.DefaultManager.CalculateDefaults(InstanceDefaultManager.GetMachineName(), "Blue");
            Assert.IsNotEmpty(defaults[typeof (Rule).FullName]);
        }

        [Test]
        public void HasADefaultInstanceKey()
        {
            InstanceManager manager = new InstanceManager(_graph);
            manager.SetDefaultsToProfile("Green");

            string defaultKey = manager[typeof (Rule)].DefaultInstanceKey;
            Assert.IsNotEmpty(defaultKey);
        }

        [Test]
        public void SetTheProfile()
        {
            InstanceManager manager = new InstanceManager(_graph);
            manager.SetDefaultsToProfile("Green");

            ColorRule greenRule = (ColorRule) manager.CreateInstance(typeof (Rule));
            Assert.AreEqual("Green", greenRule.Color);

            manager.SetDefaultsToProfile("Blue");

            ColorRule blueRule = (ColorRule) manager.CreateInstance(typeof (Rule));
            Assert.AreEqual("Blue", blueRule.Color);
        }

        [Test]
        public void GotTheInstanceForTheMachineOverride()
        {
            MachineOverride machine = _graph.DefaultManager.GetMachineOverride("SERVER");
            Assert.AreEqual(1, machine.InnerDefaults.Length);
            Assert.IsTrue(machine.HasOverride(typeof (IWidget).FullName));

            InstanceManager manager = new InstanceManager(_graph);
            string instanceKey = machine[typeof (IWidget).FullName];

            ColorWidget orange = (ColorWidget) manager.CreateInstance(typeof (IWidget), instanceKey);
            Assert.AreEqual("Orange", orange.Color);
        }

        [Test]
        public void GettingTheRightInstanceKeyWhenUsingAMAchineOverrideInCombinationWithProfile()
        {
            MachineOverride machine = _graph.DefaultManager.GetMachineOverride("SERVER");
            InstanceManager manager = new InstanceManager(_graph);
            string machineKey = machine[typeof (IWidget).FullName];

            Profile profile = manager.DefaultManager.CalculateDefaults("SERVER", "");
            string profileKey = profile[typeof (IWidget).FullName];

            Assert.AreEqual(machineKey, profileKey);
        }


        [Test]
        public void InlineMachine1()
        {
            InstanceManager manager = new InstanceManager(_graph);
            Profile profile = manager.DefaultManager.CalculateDefaults("SERVER", "");
            manager.SetDefaults(profile);

            string instanceKey = profile[typeof (IWidget).FullName];

            ColorWidget orange = (ColorWidget) manager.CreateInstance(typeof (IWidget), instanceKey);
            Assert.AreEqual("Orange", orange.Color);

            ColorWidget widget = (ColorWidget) manager.CreateInstance(typeof (IWidget));
            Assert.AreEqual("Orange", widget.Color);
        }

        [Test]
        public void InlineMachine2()
        {
            InstanceManager manager = new InstanceManager(_graph);
            Profile profile = manager.DefaultManager.CalculateDefaults("GREEN-BOX", "");
            manager.SetDefaults(profile);

            ColorWidget widget = (ColorWidget) manager.CreateInstance(typeof (IWidget));
            Assert.AreEqual("Green", widget.Color);
        }
    }
}