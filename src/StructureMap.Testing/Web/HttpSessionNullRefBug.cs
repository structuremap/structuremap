using StructureMap.Testing.Widget3;
using StructureMap.Web;
using Xunit;

namespace StructureMap.Testing.Web
{
    public class HttpSessionNullRefBug
    {
        [Fact]
        public void SetUp()
        {
            var container = new Container(x => x.For<IGateway>(WebLifecycles.HybridSession).Use<DefaultGateway>());

            container.GetInstance<IGateway>().ShouldNotBeNull();
        }
    }
}