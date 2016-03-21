using Shouldly;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class closed_type_generic_is_in_get_all
    {
        public interface IHandle<T>
        {
        }

        public class NulloHandle<T> : IHandle<T>
        {
        }

        public class MyEvent
        {
        }

        public class MyEventHandler : IHandle<MyEvent>
        {
        }

        [Fact]
        public void works_just_fine()
        {
            var container = new Container(_ =>
            {
                _.For(typeof(IHandle<>)).Use(typeof(NulloHandle<>));
                _.For<IHandle<MyEvent>>().Use<MyEventHandler>();
            });

            container.GetAllInstances<IHandle<MyEvent>>()
                .Single()
                .ShouldBeOfType<MyEventHandler>();
        }
    }
}