using NUnit.Framework;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class HttpSessionNullRefBug
    {
        [Test]
        public void SetUp()
        {
            var container = new Container(x =>
            {
                x.ForRequestedType<IGateway>()
                    .CacheBy(InstanceScope.HybridHttpSession)
                    .TheDefaultIsConcreteType<DefaultGateway>();
            });

            container.GetInstance<IGateway>().ShouldNotBeNull();
        }

        
    }
}