using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_295_named_registrations_to_nested_container
    {
        [Test]
        public void named_registrations_should_be_isolated_on_child_containers()
        {
            var container = new Container(_ => {
                _.For<IThing>().Add<Thing1>().Named("A");
            });

            var child1 = container.CreateChildContainer();
            child1.Configure(_ => _.For<IThing>().Add<Thing2>().Named("A"));
            child1.GetInstance<IThing>("A")
                .ShouldBeOfType<Thing2>();

            // parent container still uses its registration
            container.GetInstance<IThing>("A").ShouldBeOfType<Thing1>();

            var child2 = container.CreateChildContainer();

            // Should match the parent type
            child2.GetInstance<IThing>("A").ShouldBeOfType<Thing1>();
        }


        [Test]
        public void named_registrations_should_be_isolated_on_nested_container()
        {
            var container = new Container(_ =>
            {
                _.For<IThing>().Add<Thing1>().Named("A");
            });

            var nested1 = container.GetNestedContainer();
            nested1.Configure(_ => _.For<IThing>().Add<Thing2>().Named("A"));
            nested1.GetInstance<IThing>("A")
                .ShouldBeOfType<Thing2>();

            // parent container still uses its registration
            container.GetInstance<IThing>("A").ShouldBeOfType<Thing1>();

            var nested2 = container.GetNestedContainer();

            // Should match the parent type
            nested2.GetInstance<IThing>("A").ShouldBeOfType<Thing1>();
        }

        public interface IThing{}
        public class Thing1 : IThing{}
        public class Thing2 : IThing{}
        public class Thing3 : IThing{}
        public class Thing4 : IThing{}
        public class Thing5 : IThing{}
    }
}