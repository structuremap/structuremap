using StructureMap.Testing.Acceptance;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_461_Child_Container_to_Nested_Container_Singleton_Bug
    {
        [Fact]
        public void child_and_nested_container_usage_of_singletons()
        {
            var container = new Container();
            var child = container.CreateChildContainer();
            child.Configure(_ => { _.ForSingletonOf<IColorCache>().Use<ColorCache>(); });

            var singleton = child.GetInstance<IColorCache>();

            // SingletonThing's should be resolved from the child container
            using (var nested = child.GetNestedContainer())
            {
                // Fails, nested gets it's own instance
                Assert.Same(singleton, nested.GetInstance<IColorCache>());
            }
        }
    }
}