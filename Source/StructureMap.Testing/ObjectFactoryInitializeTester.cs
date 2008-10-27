using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;
using StructureMap.Testing.Widget5;

namespace StructureMap.Testing
{
    public class InitializeRegistry : Registry
    {
        public InitializeRegistry()
        {
            InstanceOf<IWidget>().Is.OfConcreteType<ColorWidget>().WithCtorArg("color").EqualTo("Green").WithName(
                "Green");
        }
    }

    [TestFixture]
    public class ObjectFactoryInitializeTester
    {
        [Test]
        public void PullConfigurationFromTheAppConfig()
        {
            ObjectFactory.Initialize(x =>
            {
                x.UseDefaultStructureMapConfigFile = false;

                // Tell StructureMap to look for configuration 
                // from the App.config file
                // The default is false
                x.PullConfigurationFromAppConfig = true;
            });

            ObjectFactory.GetInstance<IThing<string, bool>>()
                .IsType<ColorThing<string, bool>>().Color.ShouldEqual("Cornflower");
        }


        [Test]
        public void Add_a_registry_by_generic_signature()
        {
            ObjectFactory.Initialize(x =>
            {
                x.IgnoreStructureMapConfig = true;
                x.AddRegistry<InitializeRegistry>();
            });

            ObjectFactory.GetNamedInstance<IWidget>("Green").ShouldBeOfType<ColorWidget>().Color.ShouldEqual("Green");
        }

        [Test]
        public void StructureMap_functions_without_StructureMapconfig_file_in_the_default_mode()
        {
            DataMother.RemoveStructureMapConfig();

            ObjectFactory.Initialize(x => { });
        }


        [Test]
        public void TheDefaultNameIs_should_set_the_default_profile_name()
        {
            string theDefaultProfileName = "the default profile";

            ObjectFactory.Initialize(x =>
            {
                x.CreateProfile(theDefaultProfileName).For<IGateway>().Use(() => null);

                x.IgnoreStructureMapConfig = true;
                x.DefaultProfileName = theDefaultProfileName;
            });

            ObjectFactory.Profile.ShouldEqual(theDefaultProfileName);
        }
    }
}