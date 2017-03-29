using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_557_unique_instances_when_using_with
    {
        [Fact]
        public void should_return_unique_instances_when_using_With()
        {
            var root = new Container();
            var nested1 = root.GetNestedContainer();
            var nested2 = root.GetNestedContainer();

            var i1_1 = nested1.With(new Arg()).GetInstance<Target>();
            var i1_2 = nested1.With(new Arg()).GetInstance<Target>();
            i1_1.ShouldNotBeTheSameAs(i1_2);

            // Make sure that normal lifecycle was not affected by the other container
            var i2_1 = nested2.GetInstance<Target>();
            var i2_2 = nested2.GetInstance<Target>();
            i2_1.ShouldBeTheSameAs(i2_2);
        }

        public class Target
        {
            public Target(Arg arg) {}
        }

        public class Arg { }
    }
}