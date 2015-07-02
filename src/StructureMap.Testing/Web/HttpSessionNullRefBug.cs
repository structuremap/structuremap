using NUnit.Framework;
using StructureMap.Testing.Widget3;
using StructureMap.Web;

namespace StructureMap.Testing.Web
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