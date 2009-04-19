using NUnit.Framework;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Bugs
{
    [TestFixture] public class MixedConfigureAndInitializeMissingInstanceProblem
    {
        private Container container;

        [SetUp] public void SetUp()
        {
            container = new Container(x =>
            {
                x.ForRequestedType<IWidget>().MissingNamedInstanceIs.Conditional(o =>
                {
                    o.TheDefault.Is.ConstructedBy(c => new ColorWidget(c.RequestedName));
                });
            });
        }

        [Test] public void configure_again_and_try_to_fetch_the_missing_instance()
        {
            container.Configure(x =>
            {
                x.ForRequestedType<IWidget>().TheDefaultIsConcreteType<AWidget>();
            });

            container.GetInstance<IWidget>("Red").ShouldBeOfType<ColorWidget>().Color.ShouldEqual("Red");
        }

        [Test] public void configure_the_missing_method_instance_in_the_configure()
        {
            container = new Container(x =>
            {
                x.ForRequestedType<IWidget>().TheDefaultIsConcreteType<AWidget>();
            });

            container.Configure(x =>
            {
                x.ForRequestedType<IWidget>().MissingNamedInstanceIs.Conditional(o =>
                {
                    o.TheDefault.Is.ConstructedBy(c => new ColorWidget(c.RequestedName));
                });
            });

            container.GetInstance<IWidget>("Red").ShouldBeOfType<ColorWidget>().Color.ShouldEqual("Red");
        }
    }
}