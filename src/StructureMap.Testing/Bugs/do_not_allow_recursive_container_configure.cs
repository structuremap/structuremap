using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class do_not_allow_recursive_container_configure
    {
        [Fact]
        public void do_not_allow()
        {
            var container = new Container();
            Exception<StructureMapConfigurationException>.ShouldBeThrownBy(
                () =>
                {
                    container.Configure(x => container.Configure(y => { }));
                });
        }
    }
}