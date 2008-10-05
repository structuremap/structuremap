using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ProfileManagerMergeTester
    {
        private const string PROFILE = "profile";

        [Test]
        public void Import_a_default_for_a_profile_in_destination_will_override_existing_default_in_that_profile()
        {
            var source = new ProfileManager();
            var sourceInstance = new ConfiguredInstance(typeof (AWidget));
            source.SetDefault(PROFILE, typeof (IWidget), sourceInstance);

            // Fill in value before the ImportFrom
            var destination = new ProfileManager();
            var destinationInstance = new LiteralInstance(new AWidget());
            destination.SetDefault(PROFILE, typeof (IWidget), destinationInstance);

            destination.ImportFrom(source);


            Assert.AreSame(sourceInstance, destination.GetDefault(typeof (IWidget), PROFILE));
        }

        [Test]
        public void Import_a_default_when_the_destination_already_has_an_active_profile()
        {
            var source = new ProfileManager();
            var sourceInstance = new ConfiguredInstance(typeof (AWidget));
            source.SetDefault(PROFILE, typeof (IWidget), sourceInstance);

            var destination = new ProfileManager();
            destination.SetDefault(PROFILE, typeof (Rule), new ConfiguredInstance(typeof (ColorRule)));
            destination.CurrentProfile = PROFILE;
            destination.ImportFrom(source);

            Assert.AreSame(sourceInstance, destination.GetDefault(typeof (IWidget)));
        }

        [Test]
        public void Import_an_instance_from_the_default_profile()
        {
            var source = new ProfileManager();
            var sourceInstance = new ConfiguredInstance(typeof (AWidget));
            source.SetDefault(PROFILE, typeof (IWidget), sourceInstance);

            var destination = new ProfileManager();
            destination.DefaultProfileName = PROFILE;
            destination.ImportFrom(source);

            Assert.AreSame(sourceInstance, destination.GetDefault(typeof (IWidget)));
        }

        [Test]
        public void Import_the_default_does_not_impact_the_source()
        {
            var source = new ProfileManager();
            var sourceInstance = new ConfiguredInstance(typeof (AWidget));
            source.SetDefault(typeof (IWidget), sourceInstance);

            var destination = new ProfileManager();
            destination.ImportFrom(source);
            destination.SetDefault(typeof (IWidget), new LiteralInstance(new AWidget()));

            Assert.AreSame(sourceInstance, source.GetDefault(typeof (IWidget)));
        }

        [Test]
        public void Import_the_default_for_a_profile_make_sure_overriding_the_destination_does_not_impact_the_source()
        {
            var source = new ProfileManager();
            var profileInstance = new ConfiguredInstance(typeof (AWidget));
            source.SetDefault(PROFILE, typeof (IWidget), profileInstance);

            var destination = new ProfileManager();

            destination.ImportFrom(source);

            // Source should be unchanged when destination IS changed
            destination.SetDefault(PROFILE, typeof (IWidget), new LiteralInstance(new AWidget()));
            Assert.AreSame(profileInstance, source.GetDefault(typeof (IWidget), PROFILE));
        }

        [Test]
        public void Import_the_default_for_a_profile_that_is_not_in_the_destination()
        {
            var source = new ProfileManager();
            var profileInstance = new ConfiguredInstance(typeof (AWidget));
            source.SetDefault(PROFILE, typeof (IWidget), profileInstance);

            var destination = new ProfileManager();

            destination.ImportFrom(source);

            Assert.AreSame(profileInstance, destination.GetDefault(typeof (IWidget), PROFILE));
        }

        [Test]
        public void Import_the_default_if_it_is_completely_missing()
        {
            var source = new ProfileManager();
            var sourceInstance = new ConfiguredInstance(typeof (AWidget));
            source.SetDefault(typeof (IWidget), sourceInstance);

            var destination = new ProfileManager();
            destination.ImportFrom(source);

            Assert.AreSame(sourceInstance, destination.GetDefault(typeof (IWidget)));
        }
    }
}