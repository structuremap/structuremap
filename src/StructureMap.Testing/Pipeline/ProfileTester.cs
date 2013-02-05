using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Graph;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ProfileTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _profile = new Profile("something");
            _pluginGraph = new PluginGraph();
        }

        #endregion

        private Profile _profile;
        private PluginGraph _pluginGraph;

        private void setDefault<T, U>(string key) where U : T, new()
        {
            PluginFamily family = _pluginGraph.FindFamily(typeof (T));
            ConfiguredInstance instance = new ConfiguredInstance(typeof (U)).Named(key);
            family.AddInstance(instance);

            _profile.SetDefault(typeof (T), instance);
        }

        private void assertThatMasterInstanceWasFound<T>(string name)
        {
            Instance instance = _profile.GetDefault(typeof (T));
            instance.ShouldNotBeOfType(typeof (ReferencedInstance));
            Assert.AreEqual(name, instance.Name);
        }

        public interface IConstraint
        {
            
        }

        public class FakeConstraint : IConstraint
        {
            #region IConstraint Members

            public void WriteMessageTo(MessageWriter writer)
            {
                throw new NotImplementedException();
            }

            public bool Matches(object actual)
            {
                throw new NotImplementedException();
            }

            public void WriteDescriptionTo(MessageWriter writer)
            {
                throw new NotImplementedException();
            }

            public void WriteActualValueTo(MessageWriter writer)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        [Test]
        public void A_call_to_fill_is_ignored_if_there_is_already_a_default_for_that_type()
        {
            var profile = new Profile("something");
            profile.SetDefault(typeof (ISomething), new ConfiguredInstance(typeof (SomethingOne)).Named("Red"));
            profile.FillTypeInto(typeof (ISomething), new ConfiguredInstance(typeof (SomethingOne)).Named("Blue"));

            Assert.AreEqual("Red", profile.GetDefault(typeof (ISomething)).Name);
        }

        [Test]
        public void A_call_to_fill_sets_the_default_for_a_plugin_type_if_no_previous_default_is_known()
        {
            var profile = new Profile("something");
            profile.FillTypeInto(typeof (ISomething), new ConfiguredInstance(typeof (SomethingOne)).Named("Blue"));

            Assert.AreEqual("Blue", profile.GetDefault(typeof (ISomething)).Name);
        }

        [Test]
        public void CopyDefaultsFromOneTypeToAnother()
        {
            setDefault<ISomething, SomethingOne>("Red");
            _pluginGraph.FindFamily(typeof (ILifecycle)).AddInstance(
                new ConfiguredInstance(typeof (SingletonLifecycle)).Named("Red"));

            _profile.CopyDefault(typeof (ISomething), typeof (ILifecycle),
                                 _pluginGraph.FindFamily(typeof (ILifecycle)));
            _profile.GetDefault(typeof (ILifecycle)).Name.ShouldEqual("Red");
        }

        [Test]
        public void do_not_blow_up_if_the_profile_does_not_have_the_plugin_type_being_removed()
        {
            var profile = new Profile("something");

            ConfiguredInstance instance = new ConfiguredInstance(typeof (SomethingOne)).Named("Red");

            profile.Remove(typeof (ISomething), instance);
        }

        [Test]
        public void Do_not_blow_up_when_you_copy_defaults_for_a_source_type_that_does_not_exist()
        {
            _profile.CopyDefault(typeof (ISomething), typeof (ILifecycle), new PluginFamily(typeof (ISomething)));
        }

        [Test]
        public void FillAll_pushes_in_all_types()
        {
            var source = new Profile("Source");
            source.SetDefault(typeof (ISomething), new ObjectInstance(new SomethingOne()).Named("Red"));
            source.SetDefault(typeof (string), new ObjectInstance(new SomethingOne()).Named("Red"));
            source.SetDefault(typeof (int), new ObjectInstance(new SomethingOne()).Named("Red"));
            source.SetDefault(typeof (bool), new ObjectInstance(new SomethingOne()).Named("Red"));

            var destination = new Profile("Destination");
            destination.SetDefault(typeof (string), new ObjectInstance(new SomethingOne()).Named("Blue"));
            destination.SetDefault(typeof (int), new ObjectInstance(new SomethingOne()).Named("Blue"));

            source.FillAllTypesInto(destination);

            Assert.AreEqual("Red", destination.GetDefault(typeof (ISomething)).Name);
            Assert.AreEqual("Blue", destination.GetDefault(typeof (string)).Name);
            Assert.AreEqual("Blue", destination.GetDefault(typeof (int)).Name);
            Assert.AreEqual("Red", destination.GetDefault(typeof (bool)).Name);
        }

        [Test]
        public void FindMasterInstances_finds_the_master_copies_of_all()
        {
            setDefault<ISomething, SomethingOne>("Red");
            setDefault<ILifecycle, SingletonLifecycle>("Green");
            setDefault<IConstraint, FakeConstraint>("Blue");

            _profile.FindMasterInstances(_pluginGraph);

            assertThatMasterInstanceWasFound<ISomething>("Red");
            assertThatMasterInstanceWasFound<ILifecycle>("Green");
            assertThatMasterInstanceWasFound<IConstraint>("Blue");
        }

        [Test]
        public void FindMasterInstances_sad_path()
        {
            TestUtility.AssertErrorIsLogged(196, graph =>
            {
                string theInstanceName = "something";

                var profile = new Profile("something");
                profile.SetDefault(typeof (IGateway), new ReferencedInstance(theInstanceName));

                profile.FindMasterInstances(graph);
            });
        }

        [Test]
        public void remove_if_the_instance_does_match()
        {
            var profile = new Profile("something");
            ConfiguredInstance instance = new ConfiguredInstance(typeof (SomethingOne)).Named("Red");
            profile.SetDefault(typeof (ISomething), instance);

            profile.Remove(typeof (ISomething), instance);

            profile.GetDefault(typeof (ISomething)).ShouldBeNull();
        }

        [Test]
        public void remove_if_the_instance_does_not_match()
        {
            var profile = new Profile("something");
            ConfiguredInstance instance = new ConfiguredInstance(typeof (SomethingOne)).Named("Red");
            profile.SetDefault(typeof (ISomething), instance);

            profile.Remove(typeof (ISomething), new NullInstance());

            profile.GetDefault(typeof (ISomething)).ShouldBeTheSameAs(instance);
        }
    }
}