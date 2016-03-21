using Shouldly;
using StructureMap.Building;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class RedirectTester
    {
        [Fact]
        public void fail_with_cast_failure_when_the_types_are_not_convertible()
        {
            Exception<StructureMapBuildException>.ShouldBeThrownBy(() =>
            {
                var container = new Container(x =>
                {
                    x.For<ITarget>().Use<ClassThatOnlyImplementsITarget>();
                    x.Redirect<IOtherTarget, ITarget>();
                });

                container.GetInstance<IOtherTarget>().ShouldBeOfType<ClassThatImplementsBoth>();
            });
        }

        [Fact]
        public void successfully_redirect_from_one_type_to_another()
        {
            var container = new Container(x =>
            {
                x.For<ITarget>().Use<ClassThatImplementsBoth>();
                x.Redirect<IOtherTarget, ITarget>();
            });

            container.GetInstance<IOtherTarget>().ShouldBeOfType<ClassThatImplementsBoth>();
        }
    }

    public interface ITarget
    {
    }

    public interface IOtherTarget
    {
    }

    public class ClassThatImplementsBoth : ITarget, IOtherTarget
    {
    }

    public class ClassThatOnlyImplementsITarget : ITarget
    {
    }
}