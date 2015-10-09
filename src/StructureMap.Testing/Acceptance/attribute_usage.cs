using NUnit.Framework;
using Shouldly;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class attribute_usage
    {
        [Test]
        public void family_type_marked_as_singleton()
        {
            var container = new Container(_ =>
            {
                _.For<ITeamCache>().Use<TeamCache>();
                _.For<ITeam>().Use<Chargers>();
            });

            // ITeamCache is marked with [Singleton]
            container.Model.For<ITeamCache>().Lifecycle
                .ShouldBeOfType<SingletonLifecycle>();

            // ITeam is NOT marked with [Singleton]
            container.Model.For<ITeam>().Lifecycle
                .ShouldNotBeOfType<SingletonLifecycle>();
        }

        [Test]
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

        [Singleton]
        public interface ITeamCache { }
        public class TeamCache : ITeamCache { }

        public interface ITeam { }

        public class Chargers : ITeam { }

        [Singleton]
        public class Chiefs : ITeam { }




        public interface ILogger { }

        [AlwaysUnique]
        public class NLogLogger : ILogger { }

        [Test]
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
    }

    
}