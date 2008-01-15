using NUnit.Framework;
using StructureMap.Graph;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class OverrideGraphTester
    {
        [Test]
        public void CalculatesAnAggregateOfTypes()
        {
            InstanceDefaultManager defaultManager = new InstanceDefaultManager();

            defaultManager.AddPluginFamilyDefault("Type1", "PluginFamilyDefault1");
            defaultManager.AddPluginFamilyDefault("Type2", "PluginFamilyDefault2");
            defaultManager.AddPluginFamilyDefault("Type3", "PluginFamilyDefault3");
            defaultManager.AddPluginFamilyDefault("Type4", "PluginFamilyDefault4");

            MachineOverride machine = new MachineOverride("Machine1");
            machine.AddMachineOverride("Type2", "MachineSpecificKey2");
            machine.AddMachineOverride("Type3", "MachineSpecificKey3");
            defaultManager.AddMachineOverride(machine);

            Profile configuredProfile = new Profile("Production");
            configuredProfile.AddOverride("Type3", "ProfileDefault3");
            configuredProfile.AddOverride("Type4", "ProfileDefault4");
            defaultManager.AddProfile(configuredProfile);


            Profile profile = defaultManager.CalculateDefaults("Machine1", "Production");
            Assert.AreEqual(4, profile.Count);
            Assert.AreEqual("PluginFamilyDefault1", profile["Type1"]);
            Assert.AreEqual("MachineSpecificKey2", profile["Type2"]);
            Assert.AreEqual("ProfileDefault3", profile["Type3"]);
            Assert.AreEqual("ProfileDefault4", profile["Type4"]);
        }

        [Test]
        public void FindDefaultsWithOnlyPluginFamilyDefaults()
        {
            InstanceDefaultManager defaultManager = new InstanceDefaultManager();
            defaultManager.AddPluginFamilyDefault(new InstanceDefault("Type1", "PluginFamilyDefault"));

            Profile profile = defaultManager.CalculateDefaults(string.Empty, string.Empty);
            Assert.AreEqual(1, profile.Count);
            Assert.AreEqual("PluginFamilyDefault", profile["Type1"]);
        }

        [Test]
        public void FindDefaultsWithPluginFamilyDefaultAndAMatchingMachineOverridesDefaultKey()
        {
            InstanceDefaultManager defaultManager = new InstanceDefaultManager();
            defaultManager.AddPluginFamilyDefault("Type1", "PluginFamilyDefault");
            MachineOverride machine = new MachineOverride("Machine1");
            machine.AddMachineOverride("Type1", "MachineSpecificKey");
            defaultManager.AddMachineOverride(machine);

            Profile profile = defaultManager.CalculateDefaults("Machine1", string.Empty);
            Assert.AreEqual(1, profile.Count);
            Assert.AreEqual("MachineSpecificKey", profile["Type1"]);
        }

        [Test]
        public void FindDefaultsWithPluginFamilyDefaultsAndAMachineNameThatDoesNotMatch()
        {
            InstanceDefaultManager defaultManager = new InstanceDefaultManager();
            defaultManager.AddPluginFamilyDefault("Type1", "PluginFamilyDefault");
            MachineOverride machine = new MachineOverride("Machine1");
            machine.AddMachineOverride("Type1", "MachineSpecificKey");

            Profile profile = defaultManager.CalculateDefaults("Machine2", string.Empty);
            Assert.AreEqual(1, profile.Count);
            Assert.AreEqual("PluginFamilyDefault", profile["Type1"]);
        }

        [Test]
        public void FindsDefaultWhenOverridenByProfileWithAndWithoutAMatchingMachineOverride()
        {
            InstanceDefaultManager defaultManager = new InstanceDefaultManager();
            defaultManager.DefaultProfileName = "SomethingElse";

            defaultManager.AddPluginFamilyDefault("Type1", "PluginFamilyDefault");
            Profile configuredProfile = new Profile("Production");
            configuredProfile.AddOverride("Type1", "ProfileDefault");
            defaultManager.AddProfile(configuredProfile);

            Profile profile = defaultManager.CalculateDefaults("Machine1", "Production");
            Assert.AreEqual(1, profile.Count);
            Assert.AreEqual("ProfileDefault", profile["Type1"]);

            // Add the machine override, get the same results
            MachineOverride machine = new MachineOverride("Machine1");
            machine.AddMachineOverride("Type1", "MachineSpecificKey");
            defaultManager.AddMachineOverride(machine);

            profile = defaultManager.CalculateDefaults("Machine1", "Production");
            Assert.AreEqual(1, profile.Count);
            Assert.AreEqual("ProfileDefault", profile["Type1"]);
        }

        [Test]
        public void MachineOverrideGetsTheCorrectAnswerWithAndWithoutAProfile()
        {
            MachineOverride machine = new MachineOverride("Machine1");
            machine.AddMachineOverride("Type1", "MachineSpecificKey1");
            machine.AddMachineOverride("Type2", "MachineSpecificKey2");

            Assert.AreEqual("MachineSpecificKey1", machine["Type1"]);
            Assert.AreEqual("MachineSpecificKey2", machine["Type2"]);
            Assert.IsFalse(machine.HasOverride("Type3"));

            Profile profile = new Profile("Production");
            profile.AddOverride("Type2", "ProfileDefault2");
            profile.AddOverride("Type3", "ProfileDefault3");
            machine = new MachineOverride("Machine1", profile);
            machine.AddMachineOverride("Type1", "MachineSpecificKey1");
            machine.AddMachineOverride("Type2", "MachineSpecificKey2");


            Assert.IsTrue(machine.HasOverride("Type3"));
            Assert.AreEqual("MachineSpecificKey1", machine["Type1"]);
            Assert.AreEqual("ProfileDefault2", machine["Type2"]);
            Assert.AreEqual("ProfileDefault3", machine["Type3"]);
        }

        [Test]
        public void UsesTheDefaultProfile()
        {
            Profile defaultProfile = new Profile("Default");
            defaultProfile.AddOverride("type1", "default");
            defaultProfile.AddOverride("type2", "default");
            defaultProfile.AddOverride("type3", "default");

            Profile otherProfile = new Profile("Other");
            otherProfile.AddOverride("type1", "other");
            otherProfile.AddOverride("type2", "other");
            otherProfile.AddOverride("type3", "other");

            InstanceDefaultManager manager = new InstanceDefaultManager();
            manager.AddPluginFamilyDefault("type1", "family");
            manager.AddPluginFamilyDefault("type2", "family");
            manager.AddPluginFamilyDefault("type3", "family");

            manager.AddProfile(defaultProfile);
            manager.AddProfile(otherProfile);

            manager.DefaultProfileName = defaultProfile.ProfileName;

            Profile actual = manager.CalculateDefaults(string.Empty, string.Empty);

            Assert.AreEqual("default", actual["type1"]);
            Assert.AreEqual("default", actual["type2"]);
            Assert.AreEqual("default", actual["type3"]);

            actual = manager.CalculateDefaults(string.Empty, "Other");
            Assert.AreEqual("other", actual["type1"]);
            Assert.AreEqual("other", actual["type2"]);
            Assert.AreEqual("other", actual["type3"]);
        }
    }
}