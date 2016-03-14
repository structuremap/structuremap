using StructureMap.Testing.Widget;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class AddValueDirectlyWithGenericUsage
    {
        // SAMPLE: injecting-pre-built-object
        [Fact]
        public void should_be_able_to_resolve_from_the_generic_family_expression()
        {
            var widget = new AWidget();
            var container = new Container(x => x.For(typeof(IWidget)).Use(widget).Named("mine"));

            container.GetInstance<IWidget>("mine").ShouldBeTheSameAs(widget);
        }

        // ENDSAMPLE
    }
}