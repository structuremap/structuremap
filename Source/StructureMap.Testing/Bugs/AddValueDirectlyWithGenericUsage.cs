using NUnit.Framework;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class AddValueDirectlyWithGenericUsage
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void should_be_able_to_resolve_from_the_generic_family_expression()
        {
            var widget = new AWidget();
            var container = new Container(x => x.For(typeof (IWidget)).Use(widget).WithName("mine"));

            container.GetInstance<IWidget>("mine").ShouldBeTheSameAs(widget);
        }
    }
}