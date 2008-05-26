using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Configuration
{
    [TestFixture]
    public class ProfileBuilderTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _graph = new PluginGraph();
            _builder = new ProfileBuilder(_graph, THE_MACHINE_NAME);
        }

        #endregion

        private PluginGraph _graph;
        private ProfileBuilder _builder;
        private const string THE_MACHINE_NAME = "TheMachineName";

        [Test]
        public void Override_Profile_should_add_a_ReferencedInstance_to_the_ProfileManager()
        {
            _builder.AddProfile("Connected");
            _builder.OverrideProfile(new TypePath(this.GetType()), "Red");

            _builder.AddProfile("Stubbed");
            _builder.OverrideProfile(new TypePath(this.GetType()), "Blue");

            Instance connectedInstance = _graph.ProfileManager.GetDefault(this.GetType(), "Connected");
            Assert.AreEqual(new ReferencedInstance("Red"), connectedInstance);

            Instance stubbedInstance = _graph.ProfileManager.GetDefault(this.GetType(), "Stubbed");
            Assert.AreEqual(new ReferencedInstance("Blue"), stubbedInstance);
        }

        [Test]
        public void Ignore_any_information_from_a_different_machine()
        {
            _builder.AddMachine("DifferentMachine", "TheProfile");
            Assert.IsTrue(string.IsNullOrEmpty(_graph.ProfileManager.DefaultMachineProfileName));
        }

        [Test]
        public void Use_the_machine_profile_name_if_the_machine_name_matches()
        {
            _builder.AddMachine(THE_MACHINE_NAME, "TheProfile");
            Assert.AreEqual("TheProfile", _graph.ProfileManager.DefaultMachineProfileName);
        }

        [Test]
        public void Register_a_machine_override_if_it_is_the_matching_machine()
        {
            _builder.AddMachine(THE_MACHINE_NAME, "TheProfile");
            _builder.OverrideMachine(new TypePath(this.GetType()), "Purple");

            ReferencedInstance instance = new ReferencedInstance("Purple");
            Assert.AreEqual(instance, _graph.ProfileManager.GetMachineDefault(this.GetType()));
        }


        [Test]
        public void Do_not_register_a_machine_override_if_it_is_NOT_the_matching_machine()
        {
            _builder.AddMachine("Some other machine", "TheProfile");
            _builder.OverrideMachine(new TypePath(this.GetType()), "Purple");

            Assert.IsNull(_graph.ProfileManager.GetMachineDefault(this.GetType()));
        }

        [Test]
        public void SetDefaultProfileName()
        {
            string theProfileName = "some profile name";
            _builder.SetDefaultProfileName(theProfileName);

            Assert.AreEqual(theProfileName, _graph.ProfileManager.DefaultProfileName);
        }

        [Test]
        public void Override_profile_with_a_bad_TypePath_should_log_107()
        {
            PluginGraph graph = new PluginGraph();

            ProfileBuilder builder = new ProfileBuilder(graph);

            builder.AddProfile("something");
            builder.OverrideProfile(new TypePath("a type that does not exist"), "key");

            graph.Log.AssertHasError(107);
        }
    }
}