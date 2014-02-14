using FubuCore.Binding;
using FubuCore.Configuration;
using NUnit.Framework;
using StructureMap;
using StructureMap.Testing;

namespace FubuMVC.StructureMap3.Testing.Settings
{
    [TestFixture]
    public class SettingsInstanceTester
    {
        [Test]
        public void settings_instance_works()
        {
            var container = new Container(x => {
                x.For<ISettingsSource>().Add<FakeSettingsData>();
                x.For<ISettingsProvider>().Use<SettingsProvider>();
                x.For<IObjectResolver>().Use(ObjectResolver.Basic());

                x.For<FooSettings>().UseInstance(new SettingsInstance<FooSettings>());
            });

            container.GetInstance<FooSettings>()
                .Name.ShouldEqual("Max");
        }
    }

}