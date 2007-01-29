using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing.Graph
{
    /// <summary>
    /// Uses the ProfileAndMachine.XML file for configuration inputs
    /// </summary>
    [TestFixture]
    public class OverrideTester
    {
        private InstanceDefaultManager defaultManager;

        [SetUp]
        public void SetUp()
        {
            defaultManager = DataMother.GetDiagnosticPluginGraph("ProfileAndMachine.xml").DefaultManager;
        }

        [Test]
        public void DefaultsNoProfileNoMatchingMachine()
        {
            Profile defaults = defaultManager.CalculateDefaults("LOCAL", string.Empty);
            Assert.IsNotNull(defaults);

            Assert.AreEqual(string.Empty, defaults["StructureMap.Testing.Widget.Rule"], "Default Rule is string.empty");
            Assert.AreEqual("Red", defaults["StructureMap.Testing.Widget.IWidget"], "Default IWidget is Red");
        }

        [Test]
        public void DefaultsMatchingMachineOnly()
        {
            Profile defaults = defaultManager.CalculateDefaults("SERVER", string.Empty);
            Assert.IsNotNull(defaults);
            Assert.AreEqual(string.Empty, defaults["StructureMap.Testing.Widget.Rule"], "Default Rule is string.empty");
            Assert.AreEqual("Orange", defaults["StructureMap.Testing.Widget.IWidget"], "Default IWidget is Orange");
        }

        [Test]
        public void DefaultsMatchingProfileOnly()
        {
            Profile defaults = defaultManager.CalculateDefaults("LOCAL", "Blue");
            Assert.IsNotNull(defaults);
            Assert.AreEqual("Blue", defaults["StructureMap.Testing.Widget.Rule"]);
            Assert.AreEqual("Blue", defaults["StructureMap.Testing.Widget.IWidget"]);
        }

        [Test]
        public void ProfileOverridesMachineSetting()
        {
            Profile defaults = defaultManager.CalculateDefaults("SERVER", "Blue");
            Assert.IsNotNull(defaults);
            Assert.AreEqual("Blue", defaults["StructureMap.Testing.Widget.Rule"]);
            Assert.AreEqual("Blue", defaults["StructureMap.Testing.Widget.IWidget"]);
        }

        [Test]
        public void ProfileSetAtMachine()
        {
            Profile defaults = defaultManager.CalculateDefaults("GREEN-BOX", "");
            Assert.IsNotNull(defaults);
            Assert.AreEqual("Green", defaults["StructureMap.Testing.Widget.Rule"]);
            Assert.AreEqual("Green", defaults["StructureMap.Testing.Widget.IWidget"]);
        }
    }
}