using Shouldly;
using StructureMap.Pipeline;
using System.Diagnostics;
using System.Reflection;
using Xunit;

namespace StructureMap.Testing.Acceptance
{
    public class attribute_usage
    {
        [Fact]
        public void family_type_marked_as_singleton()
        {
            var container = new Container(_ =>
            {
                _.For<ITeamCache>().Use<TeamCache>();

                // additional ITeamCache
                _.For<ITeamCache>().Add<OtherTeamCache>();

                _.For<ITeam>().Use<Chargers>();
            });

            // ITeamCache is marked with [Singleton], so all instances
            // should be singletons
            container.Model.For<ITeamCache>()
                .Instances.Each(instance =>
                {
                    instance.Lifecycle
                        .ShouldBeOfType<SingletonLifecycle>();
                });

            // ITeam is NOT marked with [Singleton]
            container.Model.For<ITeam>().Lifecycle
                .ShouldNotBeOfType<SingletonLifecycle>();
        }

        [Fact]
        public void plugged_type_marked_as_singleton()
        {
            var container = new Container(_ =>
            {
                _.For<ITeamCache>().Use<TeamCache>();
                _.For<ITeam>().Use<Chiefs>();
            });

            container.Model.For<ITeam>()
                .Default
                .Lifecycle.ShouldBeOfType<SingletonLifecycle>();
        }

        // SAMPLE: [Singleton]-usage
        [Singleton] // ALL Instance's of ITeamCache will be singletons by default
        public interface ITeamCache { }

        public class TeamCache : ITeamCache { }

        public class OtherTeamCache : ITeamCache { }

        public interface ITeam { }

        public class Chargers : ITeam { }

        [Singleton] // This specific type will be a singleton
        public class Chiefs : ITeam { }

        // ENDSAMPLE

        public interface ILogger { }

        [AlwaysUnique]
        public class NLogLogger : ILogger { }

        [Fact]
        public void using_the_always_unique_attribute()
        {
            var container = new Container(_ =>
            {
                _.For<ILogger>().Use<NLogLogger>();
            });

            container.Model.For<ILogger>()
                .Default.Lifecycle
                .ShouldBeOfType<UniquePerRequestLifecycle>();
        }

        // SAMPLE: AppSettingAttribute
        public class AppSettingAttribute : StructureMapAttribute
        {
            private readonly string _key;

            public AppSettingAttribute(string key)
            {
                _key = key;
            }

            public override void Alter(IConfiguredInstance instance, PropertyInfo property)
            {
                var value = System.Configuration.ConfigurationManager.AppSettings[_key];

                instance.Dependencies.AddForProperty(property, value);
            }

            public override void Alter(IConfiguredInstance instance, ParameterInfo parameter)
            {
                var value = System.Configuration.ConfigurationManager.AppSettings[_key];

                instance.Dependencies.AddForConstructorParameter(parameter, value);
            }
        }

        // ENDSAMPLE

        // SAMPLE: AppSettingTarget
        public class AppSettingTarget
        {
            public string Name { get; set; }

            [AppSetting("homestate")]
            public string HomeState { get; set; }

            public AppSettingTarget([AppSetting("name")]string name)
            {
                Name = name;
            }
        }

        // ENDSAMPLE

        // SAMPLE: using_parameter_and_property_attibutes
        [Fact]
        public void using_parameter_and_property_attibutes()
        {
            System.Configuration.ConfigurationManager.AppSettings["name"] = "Jeremy";
            System.Configuration.ConfigurationManager.AppSettings["homestate"] = "Missouri";

            System.Configuration.ConfigurationManager.AppSettings["name"].ShouldBe("Jeremy");

            var container = new Container();

            var target = container.GetInstance<AppSettingTarget>();

            target.Name.ShouldBe("Jeremy");
            target.HomeState.ShouldBe("Missouri");

            Debug.WriteLine(container.Model.For<AppSettingTarget>().Default.DescribeBuildPlan());
        }

        // ENDSAMPLE
    }

    // SAMPLE: using-lifecycle-attributes
    [AlwaysUnique]
    public interface IShouldBeUnique { }

    [Singleton] // because the most wonderful thing about Tiggers is that I'm the only one....
    public class Tigger { }

    // ENDSAMPLE
}