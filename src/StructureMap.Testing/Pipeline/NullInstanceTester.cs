using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class NullInstanceTester
    {
        [Test]
        public void can_use_NullInstance_as_inline_dependency()
        {
            var container = new Container(x => {
                x.ForConcreteType<DecoratedGateway>().Configure
                    .Ctor<IGateway>().Is(new NullInstance());
            });

            container.GetInstance<DecoratedGateway>()
                .InnerGateway.ShouldBeNull();
                
        }
    }
}