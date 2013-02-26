using NUnit.Framework;
using StructureMap.Testing;

namespace StructureMap.Web.Testing
{
    [TestFixture]
    public class EjectingWithHybridScoping
    {
        [Test]
        public void does_not_blow_up()
        {
            var container = new Container(x =>
            {
                x.For<IFoo>().HybridHttpOrThreadLocalScoped().Use<Foo>();
            });

            container.GetInstance<IFoo>().ShouldNotBeNull();

            container.EjectAllInstancesOf<StructureMap.Testing.IFoo>();
        }

        public interface IFoo
        {
            
        }

        public class Foo : IFoo{}
    }
}