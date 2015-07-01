using NUnit.Framework;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class MixedConfigureAndInitializeMissingInstanceProblem
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            container =
                new Container(
                    x => {
                        x.For<IWidget>()
                            .MissingNamedInstanceIs.ConstructedBy(c => new ColorWidget(c.RequestedName));
                    });
        }

        #endregion

        private Container container;

        [Test]
        public void configure_again_and_try_to_fetch_the_missing_instance()
        {
            container.Configure(x => { x.For<IWidget>().Use<AWidget>(); });

            container.GetInstance<IWidget>("Red").ShouldBeOfType<ColorWidget>().Color.ShouldBe("Red");
        }

        [Test]
        public void configure_the_missing_method_instance_in_the_configure()
        {
            container = new Container(x => { x.For<IWidget>().Use<AWidget>(); });

            container.Configure(
                x => { x.For<IWidget>().MissingNamedInstanceIs.ConstructedBy(c => new ColorWidget(c.RequestedName)); });

            container.GetInstance<IWidget>("Red").ShouldBeOfType<ColorWidget>().Color.ShouldBe("Red");
        }
    }
}