using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ProfileManagerTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _manager = new ProfileManager();
            _pluginGraph = new PluginGraph();
        }

        #endregion

        private ProfileManager _manager;
        private PluginGraph _pluginGraph;

        private void addDefaultToPluginFamily<T>(string name)
        {
            ConstructorInstance instance = new ConstructorInstance(null).WithName(name);
            PluginFamily family = _pluginGraph.FindFamily(typeof (T));
            family.AddInstance(instance);
            family.DefaultInstanceKey = instance.Name;
        }

        private void addDefaultToProfile<T>(string profile, string name)
        {
            _manager.SetDefault(profile, typeof (T), new ReferencedInstance(name));
            PluginFamily family = _pluginGraph.FindFamily(typeof (T));
            family.AddInstance(new ConstructorInstance(null).WithName(name));
        }

        private void addDefaultToMachine<T>(string name)
        {
            ConfiguredInstance instance = new ConfiguredInstance().WithName(name);
            PluginFamily family = _pluginGraph.FindFamily(typeof (T));
            family.AddInstance(instance);

            _manager.SetMachineDefault(typeof (T), new ReferencedInstance(name));
        }

        private void assertNoDefaultForType<T>()
        {
            Assert.IsNull(_manager.GetDefault(typeof (T)));
        }

        private void assertDefaultInstanceNameIs<T>(string expected)
        {
            Instance instance = _manager.GetDefault(typeof (T));
            Assert.IsNotNull(instance, "Checking default for Type " + typeof (T).FullName);
            Assert.AreEqual(expected, instance.Name, "Checking default for Type " + typeof (T).FullName);
        }

        private void seal()
        {
            _manager.Seal(_pluginGraph);
        }

        [Test]
        public void CopyDefaults()
        {
            _manager.DefaultProfileName = string.Empty;
            _manager.CurrentProfile = string.Empty;

            addDefaultToProfile<IBuildPolicy>("TheProfile", "Profile");
            addDefaultToProfile<IBuildPolicy>("TheProfile2", "Profile2");
            _manager.SetDefault(typeof (IBuildPolicy), new ReferencedInstance("TheDefault"));

            _manager.CopyDefaults(typeof (IBuildPolicy), typeof (ISomething));

            Assert.AreSame(_manager.GetDefault(typeof (IBuildPolicy)), _manager.GetDefault(typeof (ISomething)));
            Assert.AreSame(_manager.GetDefault(typeof (IBuildPolicy), "Profile"),
                           _manager.GetDefault(typeof (ISomething), "Profile"));
            Assert.AreSame(_manager.GetDefault(typeof (IBuildPolicy), "Profile2"),
                           _manager.GetDefault(typeof (ISomething), "Profile2"));
        }


        [Test]
        public void DefaultMachineProfileNotSet_and_pickUp_default_from_family_machine()
        {
            addDefaultToProfile<ISomething>("Machine", "Red");
            addDefaultToPluginFamily<ISomething>("Blue");
            _manager.DefaultMachineProfileName = null;

            seal();

            assertDefaultInstanceNameIs<ISomething>("Blue");
        }

        [Test]
        public void Got_machine_default_machine_profile_and_family_default_the_machine_wins()
        {
            addDefaultToProfile<ISomething>("MachineProfile", "MachineProfile");
            addDefaultToPluginFamily<ISomething>("Family");
            addDefaultToMachine<ISomething>("Machine");
            _manager.DefaultMachineProfileName = "MachineProfile";

            seal();

            assertDefaultInstanceNameIs<ISomething>("Machine");
        }


        [Test]
        public void Got_profile_and_family_and_machine_and_machine_profile_so_profile_wins()
        {
            addDefaultToProfile<ISomething>("TheProfile", "Profile");
            addDefaultToProfile<ISomething>("TheMachineProfile", "MachineProfile");
            addDefaultToPluginFamily<ISomething>("Family");
            addDefaultToMachine<ISomething>("Machine");
            _manager.DefaultProfileName = "TheProfile";
            _manager.DefaultMachineProfileName = "TheMachineProfile";

            seal();

            assertDefaultInstanceNameIs<ISomething>("Profile");
        }

        [Test]
        public void Got_profile_and_family_and_machine_so_profile_wins()
        {
            addDefaultToProfile<ISomething>("TheProfile", "Profile");
            addDefaultToPluginFamily<ISomething>("Family");
            addDefaultToMachine<ISomething>("Machine");
            _manager.DefaultProfileName = "TheProfile";
            seal();

            assertDefaultInstanceNameIs<ISomething>("Profile");
        }

        [Test]
        public void Got_profile_and_family_so_profile_wins()
        {
            addDefaultToProfile<ISomething>("TheProfile", "Profile");
            addDefaultToPluginFamily<ISomething>("Family");
            _manager.DefaultProfileName = "TheProfile";
            seal();

            assertDefaultInstanceNameIs<ISomething>("Profile");
        }

        [Test]
        public void Got_profile_only_and_profile_wins()
        {
            addDefaultToProfile<ISomething>("TheProfile", "Profile");
            _manager.DefaultProfileName = "TheProfile";
            seal();

            assertDefaultInstanceNameIs<ISomething>("Profile");
        }

        [Test]
        public void Have_a_machine_default_and_a_base_default_the_machine_wins()
        {
            addDefaultToPluginFamily<ISomething>("Red");
            addDefaultToMachine<ISomething>("Blue");

            seal();

            assertDefaultInstanceNameIs<ISomething>("Blue");
        }

        [Test]
        public void Have_a_machine_default_but_not_any_other_default_for_a_type()
        {
            addDefaultToMachine<ISomething>("Blue");
            seal();

            assertDefaultInstanceNameIs<ISomething>("Blue");
        }

        [Test]
        public void Have_machine_profile_and_default_from_family_machine_profile_wins()
        {
            addDefaultToProfile<ISomething>("Machine", "MachineProfile");
            addDefaultToPluginFamily<ISomething>("Family");
            _manager.DefaultMachineProfileName = "Machine";

            seal();

            assertDefaultInstanceNameIs<ISomething>("MachineProfile");
        }

        [Test]
        public void If_the_profile_is_set_and_there_is_a_default_in_that_profile_use_the_profile_default()
        {
            addDefaultToProfile<ISomething>("TheProfile", "Profile");
            addDefaultToPluginFamily<ISomething>("Family");

            seal();

            _manager.CurrentProfile = "TheProfile";
            assertDefaultInstanceNameIs<ISomething>("Profile");
        }

        [Test]
        public void Log_280_if_the_default_profile_does_not_exist_upon_call_to_Seal()
        {
            ProfileManager manager = new ProfileManager();
            manager.DefaultProfileName = "something that doesn't exist";

            PluginGraph graph = new PluginGraph();
            manager.Seal(graph);

            graph.Log.AssertHasError(280);
        }

        [Test]
        public void Log_280_if_the_machine_default_profile_cannot_be_found()
        {
            ProfileManager manager = new ProfileManager();
            manager.DefaultMachineProfileName = "something that doesn't exist";

            PluginGraph graph = new PluginGraph();
            manager.Seal(graph);

            graph.Log.AssertHasError(280);
        }

        [Test]
        public void Only_programmatic_override_so_use_the_programmatic_override()
        {
            _manager.SetDefault(typeof (ISomething), new ConfiguredInstance().WithName("Red"));
            assertDefaultInstanceNameIs<ISomething>("Red");
        }

        [Test]
        public void PluginFamily_has_default_but_no_overrides_of_any_kind()
        {
            addDefaultToPluginFamily<ISomething>("Red");
            seal();
            assertDefaultInstanceNameIs<ISomething>("Red");
        }

        [Test]
        public void Reset_the_profile_to_empty_string_and_use_the_basic_default()
        {
            addDefaultToProfile<ISomething>("TheProfile", "Profile");
            addDefaultToPluginFamily<ISomething>("Family");

            seal();

            _manager.CurrentProfile = "TheProfile";
            _manager.CurrentProfile = string.Empty;
            assertDefaultInstanceNameIs<ISomething>("Family");
        }

        [Test]
        public void Return_null_if_there_is_no_default()
        {
            seal();
            assertNoDefaultForType<ISomething>();
        }

        [Test]
        public void Set_the_profile_but_if_profile_does_not_have_default_for_that_type_try_to_use_base_default()
        {
            addDefaultToProfile<IBuildPolicy>("TheProfile", "Profile");
            addDefaultToPluginFamily<ISomething>("Family");

            seal();

            _manager.CurrentProfile = "TheProfile";

            assertDefaultInstanceNameIs<IBuildPolicy>("Profile");
            assertDefaultInstanceNameIs<ISomething>("Family");
        }

        [Test]
        public void Can_only_add_default_once_to_the_default_profile()
        {
            LiteralInstance i1 = new LiteralInstance(1);
            LiteralInstance i2 = new LiteralInstance(2);

            ProfileManager manager = new ProfileManager();

            manager.SetDefault(typeof(IGateway), i1);
            manager.GetDefault(typeof(IGateway)).ShouldBeTheSameAs(i1);

            manager.SetDefault(typeof(IGateway), i2);
            manager.GetDefault(typeof(IGateway)).ShouldBeTheSameAs(i2);

            manager.CurrentProfile = string.Empty;

            manager.GetDefault(typeof(IGateway)).ShouldBeTheSameAs(i1);
        }
    }
}