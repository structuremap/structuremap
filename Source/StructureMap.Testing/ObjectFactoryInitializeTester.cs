using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing
{
    [TestFixture]
    public class ObjectFactoryInitializeTester
    {

        [Test]
        public void PullConfigurationFromTheAppConfig()
        {
            ObjectFactory.Initialize(x =>
            {
                x.UseDefaultStructureMapConfigFile = false;
                x.PullConfigurationFromAppConfig = true;
            });

            ObjectFactory.GetInstance<IThing<string, bool>>()
                .IsType<ColorThing<string, bool>>().Color.ShouldEqual("Cornflower");
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
