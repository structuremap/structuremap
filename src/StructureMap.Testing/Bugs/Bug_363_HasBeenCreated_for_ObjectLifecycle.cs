using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_363_HasBeenCreated_for_ObjectLifecycle
    {
        [Fact]
        public void do_not_blow_up()
        {
            var foo = new Foo();

            var container = new Container(_ =>
            {
                _.For<Foo>().Use(foo);
            });

            container.Model.For<Foo>().Default.ObjectHasBeenCreated().ShouldBeTrue();
        }
    }
}