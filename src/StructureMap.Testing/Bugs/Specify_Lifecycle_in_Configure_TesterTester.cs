using NUnit.Framework;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Specify_Lifecycle_in_Configure_TesterTester
    {
        [Test]
        public void specify_the_lifecycle_in_a_Configure_if_it_is_not_already_set()
        {
            var container = new Container(x => { });
            container.Configure(x =>
            {
                x.ForSingletonOf<IGateway>()
                    .Use<DefaultGateway>();
            });

            var gateway1 = container.GetInstance<IGateway>();
            var gateway2 = container.GetInstance<IGateway>();
            var gateway3 = container.GetInstance<IGateway>();

            gateway1.ShouldBeTheSameAs(gateway2);
            gateway1.ShouldBeTheSameAs(gateway3);
        }
    }
}