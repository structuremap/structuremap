using Shouldly;
using StructureMap.Testing.Widget;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class MixedConfigureAndInitializeMissingInstanceProblem
    {
        public MixedConfigureAndInitializeMissingInstanceProblem()
        {
            container =
                new Container(
                    x =>
                    {
                        x.For<IWidget>()
                            .MissingNamedInstanceIs.ConstructedBy(c => new ColorWidget(c.RequestedName));
                    });
        }

        private Container container;

        [Fact]
        public void configure_again_and_try_to_fetch_the_missing_instance()
        {
            container.Configure(x => { x.For<IWidget>().Use<AWidget>(); });

            container.GetInstance<IWidget>("Red").ShouldBeOfType<ColorWidget>().Color.ShouldBe("Red");
        }

        [Fact]
        public void configure_the_missing_method_instance_in_the_configure()
        {
            container = new Container(x => { x.For<IWidget>().Use<AWidget>(); });

            container.Configure(
                x => { x.For<IWidget>().MissingNamedInstanceIs.ConstructedBy(c => new ColorWidget(c.RequestedName)); });

            container.GetInstance<IWidget>("Red").ShouldBeOfType<ColorWidget>().Color.ShouldBe("Red");
        }
    }
}