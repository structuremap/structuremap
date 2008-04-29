using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Graph;
using StructureMap.Pipeline;
using Profile=StructureMap.Pipeline.Profile;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ProfileTester
    {
        private Profile _profile;
        private PluginGraph _pluginGraph;

        [SetUp]
        public void SetUp()
        {
            _profile = new Profile("something");
            _pluginGraph = new PluginGraph();
        }

        private void setDefault<T>(string key)
        {
            PluginFamily family = _pluginGraph.LocateOrCreateFamilyForType(typeof (T));
            family.AddInstance(new LiteralInstance(null).WithName(key));

            _profile.SetDefault(typeof(T), new ReferencedInstance(key));
        }

        [Test]
        public void CopyDefaultsFromOneTypeToAnother()
        {
            setDefault<ISomething>("Red");

            _profile.CopyDefault(typeof (ISomething), typeof (IBuildPolicy));

            Assert.AreSame(_profile.GetDefault(typeof(ISomething)), _profile.GetDefault(typeof(IBuildPolicy)));
        }

        [Test]
        public void Do_not_blow_up_when_you_copy_defaults_for_a_source_type_that_does_not_exist()
        {
            _profile.CopyDefault(typeof(ISomething), typeof(IBuildPolicy));
        }

        [Test]
        public void A_call_to_fill_is_ignored_if_there_is_already_a_default_for_that_type()
        {
            Profile profile = new Profile("something");
            profile.SetDefault(typeof(ISomething), new LiteralInstance(null).WithName("Red"));
            profile.FillTypeInto(typeof(ISomething), new LiteralInstance(null).WithName("Blue"));

            Assert.AreEqual("Red", profile.GetDefault(typeof(ISomething)).Name);
        }

        [Test]
        public void A_call_to_fill_sets_the_default_for_a_plugin_type_if_no_previous_default_is_known()
        {
            Profile profile = new Profile("something");
            profile.FillTypeInto(typeof(ISomething), new LiteralInstance(null).WithName("Blue"));

            Assert.AreEqual("Blue", profile.GetDefault(typeof(ISomething)).Name);
        }

        [Test]
        public void FillAll_pushes_in_all_types()
        {
            Profile source = new Profile("Source");
            source.SetDefault(typeof(ISomething), new LiteralInstance(null).WithName("Red"));
            source.SetDefault(typeof(string), new LiteralInstance(null).WithName("Red"));
            source.SetDefault(typeof(int), new LiteralInstance(null).WithName("Red"));
            source.SetDefault(typeof(bool), new LiteralInstance(null).WithName("Red"));

            Profile destination = new Profile("Destination");
            destination.SetDefault(typeof(string), new LiteralInstance(null).WithName("Blue"));
            destination.SetDefault(typeof(int), new LiteralInstance(null).WithName("Blue"));

            source.FillAllTypesInto(destination);

            Assert.AreEqual("Red", destination.GetDefault(typeof(ISomething)).Name);
            Assert.AreEqual("Blue", destination.GetDefault(typeof(string)).Name);
            Assert.AreEqual("Blue", destination.GetDefault(typeof(int)).Name);
            Assert.AreEqual("Red", destination.GetDefault(typeof(bool)).Name);
        }

        private void assertThatMasterInstanceWasFound<T>(string name)
        {
            Instance instance = _profile.GetDefault(typeof (T));
            Assert.IsNotInstanceOfType(typeof(ReferencedInstance), instance);
            Assert.AreEqual(name, instance.Name);
        }

        [Test]
        public void FindMasterInstances_finds_the_master_copies_of_all()
        {
            setDefault<ISomething>("Red");
            setDefault<IBuildPolicy>("Green");
            setDefault<IConstraint>("Blue");

            _profile.FindMasterInstances(_pluginGraph);

            assertThatMasterInstanceWasFound<ISomething>("Red");
            assertThatMasterInstanceWasFound<IBuildPolicy>("Green");
            assertThatMasterInstanceWasFound<IConstraint>("Blue");
        }
    }
}
