using StructureMap.Testing.Acceptance;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_473_singletons_in_child_containers
    {
        [Fact]
        public void should_be_the_same_in_the_nested_as_in_the_child_container()
        {
            var container = new Container();

            var child = container.CreateChildContainer();

            child.Configure(_ =>
            {
                _.ForSingletonOf<IWidget>().Use<AWidget>();
            });

            var instance1 = child.GetInstance<IWidget>();

            var instance2 = child.GetNestedContainer().GetInstance<IWidget>();

            instance1.ShouldBeTheSameAs(instance2);
        }
    }
}