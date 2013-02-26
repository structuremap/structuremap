using NUnit.Framework;
using StructureMap.Testing;
using StructureMap.Testing.Widget3;

namespace StructureMap.Web.Testing
{
    [TestFixture]
    public class HttpSessionNullRefBug
    {
        [Test]
        public void SetUp()
        {
            var container = new Container(x => x.For<IGateway>(WebLifecycles.HybridSession).Use<DefaultGateway>());

            container.GetInstance<IGateway>().ShouldNotBeNull();
        }
    }
}