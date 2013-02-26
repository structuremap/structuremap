using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class HttpSessionNullRefBug
    {
        [Test]
        public void SetUp()
        {
            var container = new Container(x => x.For<IGateway>(Lifecycles.HybridSession).Use<DefaultGateway>());

            container.GetInstance<IGateway>().ShouldNotBeNull();
        }
    }
}