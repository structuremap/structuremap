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
            _builder = new ProfileBuilder(_graph);
        }

        #endregion

        private PluginGraph _graph;
        private ProfileBuilder _builder;

        [Test]
        public void Log_131_if_trying_to_register_override_for_a_profile_when_the_PluginType_cannot_be_found()
        {
            _builder.AddProfile("TheProfile");

            _graph.Log.AssertHasNoError(131);

            _builder.OverrideProfile(new TypePath("not a real type"), "Purple");

            _graph.Log.AssertHasError(131);
        }

        [Test]
        public void Override_Profile_should_add_a_ReferencedInstance_to_the_ProfileManager()
        {
            _builder.AddProfile("Connected");
            _builder.OverrideProfile(new TypePath(GetType()), "Red");

            _builder.AddProfile("Stubbed");
            _builder.OverrideProfile(new TypePath(GetType()), "Blue");

            Instance connectedInstance = _graph.ProfileManager.GetDefault(GetType(), "Connected");
            Assert.AreEqual(new ReferencedInstance("Red"), connectedInstance);

            Instance stubbedInstance = _graph.ProfileManager.GetDefault(GetType(), "Stubbed");
            Assert.AreEqual(new ReferencedInstance("Blue"), stubbedInstance);
        }

        [Test]
        public void SetDefaultProfileName()
        {
            string theProfileName = "some profile name";
            _builder.SetDefaultProfileName(theProfileName);

            Assert.AreEqual(theProfileName, _graph.ProfileManager.DefaultProfileName);
        }

        [Test]
        public void Throw_280_if_requesting_an_invalid_profile()
        {
            try
            {
                var manager = new ProfileManager();
                manager.CurrentProfile = "some profile that does not exist";

                Assert.Fail("Should have thrown error");
            }
            catch (StructureMapException ex)
            {
                Assert.AreEqual(280, ex.ErrorCode);
            }
        }
    }
}