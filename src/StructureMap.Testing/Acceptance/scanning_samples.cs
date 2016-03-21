using Shouldly;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Acceptance
{
    public class scanning_samples
    {
        // SAMPLE: WithDefaultConventions
        public interface ISpaceship { }

        public class Spaceship : ISpaceship { }

        public interface IRocket { }

        public class Rocket : IRocket { }

        [Fact]
        public void default_scanning_in_action()
        {
            var container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.Assembly("StructureMap.Testing");
                    x.WithDefaultConventions();
                });
            });

            container.GetInstance<ISpaceship>().ShouldBeOfType<Spaceship>();
            container.GetInstance<IRocket>().ShouldBeOfType<Rocket>();
        }

        // ENDSAMPLE

        // SAMPLE: register-all-types-implementing
        public interface IFantasySeries { }

        public class WheelOfTime : IFantasySeries { }

        public class GameOfThrones : IFantasySeries { }

        public class BlackCompany : IFantasySeries { }

        [Fact]
        public void register_all_types_of_an_interface()
        {
            var container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.TheCallingAssembly();

                    x.AddAllTypesOf<IFantasySeries>();

                    // or

                    x.AddAllTypesOf(typeof(IFantasySeries))
                        .NameBy(type => type.Name.ToLower());
                });
            });

            container.Model.For<IFantasySeries>()
                .Instances.Select(x => x.ReturnedType)
                .OrderBy(x => x.Name)
                .ShouldHaveTheSameElementsAs(typeof(BlackCompany), typeof(GameOfThrones), typeof(WheelOfTime));
        }

        // ENDSAMPLE

        // SAMPLE: SingleImplementationsOfInterface
        public interface ISong { }

        public class TheOnlySong : ISong { }

        [Fact]
        public void only_implementation()
        {
            var container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.SingleImplementationsOfInterface();
                });
            });

            container.GetInstance<ISong>()
                .ShouldBeOfType<TheOnlySong>();
        }

        // ENDSAMPLE
    }
}