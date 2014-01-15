using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Building
{
    [TestFixture]
    public class array_expression_building
    {
        [Test]
        public void can_build_with_array_dependency()
        {
            var gateway1 = new StubbedGateway();
            var gateway2 = new StubbedGateway();
            var gateway3 = new StubbedGateway();

            var build = new ConcreteBuild<GatewayArrayUser>();
            var array = new ArrayDependencySource(typeof (IGateway),
                Constant.For(gateway1),
                Constant.For(gateway2),
                Constant.For(gateway3));

            build.ConstructorArgs(array);

            var arrayUser = build.Build<GatewayArrayUser>(new FakeBuildSession());

            arrayUser.Gateways.ShouldHaveTheSameElementsAs(gateway1, gateway2, gateway3);
        }

        [Test]
        public void return_type_of_ArrayDependencySource()
        {
            var gateway1 = new StubbedGateway();
            var gateway2 = new StubbedGateway();
            var gateway3 = new StubbedGateway();
            var array = new ArrayDependencySource(typeof(IGateway),
                Constant.For(gateway1),
                Constant.For(gateway2),
                Constant.For(gateway3));

            array.ReturnedType.ShouldEqual(typeof (IGateway[]));
        }

        [Test]
        public void can_build_with_ienumerable_dependency()
        {
            var gateway1 = new StubbedGateway();
            var gateway2 = new StubbedGateway();
            var gateway3 = new StubbedGateway();

            var build = new ConcreteBuild<GatewayEnumerableUser>();
            var array = new ArrayDependencySource(typeof (IGateway),
                Constant.For(gateway1),
                Constant.For(gateway2),
                Constant.For(gateway3));

            build.ConstructorArgs(array);

            var enumerableUser = build.Build<GatewayEnumerableUser>(new FakeBuildSession());

            enumerableUser.Gateways.ShouldHaveTheSameElementsAs(gateway1, gateway2, gateway3);
        }

        [Test]
        public void can_build_with_ilist_dependency()
        {
            var gateway1 = new StubbedGateway();
            var gateway2 = new StubbedGateway();
            var gateway3 = new StubbedGateway();

            var build = new ConcreteBuild<GatewayIListUser>();
            var array = new ArrayDependencySource(typeof (IGateway),
                Constant.For(gateway1),
                Constant.For(gateway2),
                Constant.For(gateway3));

            build.ConstructorArgs(array);

            var arrayUser = build.Build<GatewayIListUser>(new FakeBuildSession());

            arrayUser.Gateways.ShouldHaveTheSameElementsAs(gateway1, gateway2, gateway3);
        }
    }

    public class GatewayArrayUser
    {
        private readonly IGateway[] _gateways;

        public GatewayArrayUser(IGateway[] gateways)
        {
            _gateways = gateways;
        }

        public IGateway[] Gateways
        {
            get { return _gateways; }
        }
    }

    public class GatewayEnumerableUser
    {
        private readonly IEnumerable<IGateway> _gateways;

        public GatewayEnumerableUser(IEnumerable<IGateway> gateways)
        {
            _gateways = gateways;
        }

        public IEnumerable<IGateway> Gateways
        {
            get { return _gateways; }
        }
    }

    public class GatewayIListUser
    {
        private readonly IList<IGateway> _gateways;

        public GatewayIListUser(IList<IGateway> gateways)
        {
            _gateways = gateways;
        }

        public IEnumerable<IGateway> Gateways
        {
            get { return _gateways; }
        }
    }
}