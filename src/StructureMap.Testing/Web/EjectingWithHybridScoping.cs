using StructureMap.Web;
using Xunit;

namespace StructureMap.Testing.Web
{
    public class EjectingWithHybridScoping
    {
        [Fact]
        public void does_not_blow_up()
        {
            var container = new Container(x => { x.For<IFoo>().HybridHttpOrThreadLocalScoped().Use<Foo>(); });

            container.GetInstance<IFoo>().ShouldNotBeNull();

            container.EjectAllInstancesOf<Testing.IFoo>();
        }

        public interface IFoo
        {
        }

        public class Foo : IFoo
        {
        }
    }
}