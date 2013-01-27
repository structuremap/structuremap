using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Graph;
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
            ObjectInstance instance = new ObjectInstance(0).Named(name);
            PluginFamily family = _pluginGraph.FindFamily(typeof (T));
            family.AddInstance(instance);
            family.DefaultInstanceKey = instance.Name;
        }

        private void addDefaultToProfile<T>(string profile, string name)
        {
            _manager.SetDefault(profile, typeof (T), new ReferencedInstance(name));
            PluginFamily family = _pluginGraph.FindFamily(typeof (T));
            family.AddInstance(new ObjectInstance(0).Named(name));
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
        public void Can_only_add_default_once_to_the_default_profile()
        {
            var i1 = new ObjectInstance(1);
            var i2 = new ObjectInstance(2);

            var manager = new ProfileManager();

            manager.SetDefault(typeof (IGateway), i1);
            manager.GetDefault(typeof (IGateway)).ShouldBeTheSameAs(i1);

            manager.SetDefault(typeof (IGateway), i2);
            manager.GetDefault(typeof (IGateway)).ShouldBeTheSameAs(i2);

            manager.CurrentProfile = string.Empty;

            manager.GetDefault(typeof (IGateway)).ShouldBeTheSameAs(i1);
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
            var manager = new ProfileManager();
            manager.DefaultProfileName = "something that doesn't exist";

            var graph = new PluginGraph();
            manager.Seal(graph);

            graph.Log.AssertHasError(280);
        }

        [Test]
        public void Only_programmatic_override_so_use_the_programmatic_override()
        {
            _manager.SetDefault(typeof (ISomething), new ConfiguredInstance(typeof (SomethingOne)).Named("Red"));
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
            addDefaultToProfile<ILifecycle>("TheProfile", "Profile");
            addDefaultToPluginFamily<ISomething>("Family");

            seal();

            _manager.CurrentProfile = "TheProfile";

            assertDefaultInstanceNameIs<ILifecycle>("Profile");
            assertDefaultInstanceNameIs<ISomething>("Family");
        }
    }
}