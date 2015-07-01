using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore.Configuration;
using FubuMVC.Core;
using FubuMVC.StructureMap3.Settings;
using NUnit.Framework;
using Shouldly;
using StructureMap;
using StructureMap.Testing;

namespace FubuMVC.StructureMap3.Testing.Settings
{
    [TestFixture]
    public class SettingIntegrationTester
    {
        private FubuRegistry registry;
        private Lazy<IContainer> container;

        private FubuRuntime runtime;

        [SetUp]
        public void SetUp()
        {
            registry = new FubuRegistry();
            container = new Lazy<IContainer>(() => {
                var c = new Container(x => x.For<ISettingsSource>().Add<FakeSettingsData>());

                runtime = FubuApplication.For(registry).StructureMap(c).Bootstrap();

                return c;
            });
        }

        [TearDown]
        public void TearDown()
        {
            if (runtime != null) runtime.Dispose();
        }

        public FooSettings TheResultingSettings
        {
            get
            {
                return container.Value.GetInstance<FooSettings>();
            }
        }

        [Test]
        public void include_explicitly()
        {
            registry.AlterSettings<ConfigurationSettings>(x => {
                x.Include<FooSettings>();
            });

            TheResultingSettings.Name.ShouldBe("Max");
            TheResultingSettings.Age.ShouldBe(9);
        }

        [Test]
        public void include_by_settings_convention_in_the_application_assembly()
        {
            registry.AlterSettings<ConfigurationSettings>(x => {
                x.IncludeAllClassesSuffixedBySetting().FromTheApplicationAssembly();
            });

            container.Value.GetInstance<BarSettings>().Direction.ShouldBe("North");
            TheResultingSettings.Name.ShouldBe("Max");
            TheResultingSettings.Age.ShouldBe(9);
        }

        [Test]
        public void include_by_settings_convention_by_picking_the_assembly()
        {
            registry.AlterSettings<ConfigurationSettings>(x => {
                x.IncludeAllClassesSuffixedBySetting().FromAssembly(Assembly.GetExecutingAssembly());
            });

            container.Value.GetInstance<BarSettings>().Direction.ShouldBe("North");
            TheResultingSettings.Name.ShouldBe("Max");
            TheResultingSettings.Age.ShouldBe(9);
        }

        [Test]
        public void do_not_override_a_setting_class_that_is_configured_inside_the_fubu_registry()
        {
            registry.Services(x => x.ReplaceService(new BarSettings{Direction = "West"}));
            registry.AlterSettings<ConfigurationSettings>(x =>
            {
                x.IncludeAllClassesSuffixedBySetting().FromTheApplicationAssembly();
            });

            container.Value.GetInstance<BarSettings>().Direction.ShouldBe("West");
        }
    }

    

    public class FooSettings
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class BarSettings
    {
        public string Direction { get; set; }
    }

    public class FakeSettingsData : ISettingsSource
    {
        public IEnumerable<SettingsData> FindSettingData()
        {
            var data = new SettingsData();
            data.Child("FooSettings").Set("Name", "Max");
            data.Child("FooSettings").Set("Age", "9");
            data.Child("BarSettings").Set("Direction", "North");

            yield return data;
        }
    }
}