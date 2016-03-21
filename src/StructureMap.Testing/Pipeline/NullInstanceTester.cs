using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class NullInstanceTester
    {
        [Fact]
        public void can_use_NullInstance_as_inline_dependency()
        {
            var container = new Container(x =>
            {
                x.ForConcreteType<DecoratedGateway>().Configure
                    .Ctor<IGateway>().Is(new NullInstance());
            });

            container.GetInstance<DecoratedGateway>()
                .InnerGateway.ShouldBeNull();
        }
    }
}