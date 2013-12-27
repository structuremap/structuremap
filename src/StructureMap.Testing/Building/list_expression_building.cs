using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Building
{
    [TestFixture]
    public class list_expression_building
    {
        [Test]
        public void can_build_with_list_dependency()
        {
            var gateway1 = new StubbedGateway();
            var gateway2 = new StubbedGateway();
            var gateway3 = new StubbedGateway();

            var build = new ConcreteBuild<GatewayListUser>();
            var array = new ListDependencySource(typeof(IGateway),
                Constant.For(gateway1),
                Constant.For(gateway2),
                Constant.For(gateway3));

            build.ConstructorArgs(array);

            var arrayUser = build.Build<GatewayListUser>(new FakeBuildSession());

            arrayUser.Gateways.ShouldHaveTheSameElementsAs(gateway1, gateway2, gateway3);
        }
    }

    public class GatewayListUser
    {
        private readonly List<IGateway> _gateways;

        public GatewayListUser(List<IGateway> gateways)
        {
            _gateways = gateways;
        }

        public IEnumerable<IGateway> Gateways
        {
            get { return _gateways; }
        }
    }
}