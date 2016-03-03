using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class do_not_allow_recursive_container_configure
    {
        [Test]
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
