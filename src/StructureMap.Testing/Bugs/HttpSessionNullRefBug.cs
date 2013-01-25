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
                x.For<IGateway>(InstanceScope.HybridHttpSession)
                    .Use<DefaultGateway>();
            });

            container.GetInstance<IGateway>().ShouldNotBeNull();
        }
    }
}