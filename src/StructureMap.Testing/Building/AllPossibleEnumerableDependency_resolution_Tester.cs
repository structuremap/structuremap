using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Testing.Graph;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Building
{
    [TestFixture]
    public class AllPossibleEnumerableDependency_resolution_Tester
    {
        private FakeBuildSession theSession;
        private StubbedGateway gateway1;
        private StubbedGateway gateway2;
        private StubbedGateway gateway3;

        [SetUp]
        public void SetUp()
        {
            gateway1 = new StubbedGateway();
            gateway2 = new StubbedGateway();
            gateway3 = new StubbedGateway();

            theSession = new FakeBuildSession();
            theSession.LifecycledObjects[typeof (IGateway)][new FakeInstance()] = gateway1;
            theSession.LifecycledObjects[typeof (IGateway)][new FakeInstance()] = gateway2;
            theSession.LifecycledObjects[typeof (IGateway)][new FakeInstance()] = gateway3;
        }

        [Test]
        public void can_build_with_array_dependency()
        {
            var build = new ConcreteBuild<GatewayArrayUser>();

            build.ConstructorArgs(new AllPossibleValuesDependencySource(typeof (IGateway[])));

            var arrayUser = build.Build<GatewayArrayUser>(theSession);

            arrayUser.Gateways.ShouldHaveTheSameElementsAs(gateway1, gateway2, gateway3);
        }

        [Test]
        public void can_build_with_ienumerable_dependency()
        {
            var build = new ConcreteBuild<GatewayEnumerableUser>();

            build.ConstructorArgs(new AllPossibleValuesDependencySource(typeof (IEnumerable<IGateway>)));

            var enumerableUser = build.Build<GatewayEnumerableUser>(theSession);

            enumerableUser.Gateways.ShouldHaveTheSameElementsAs(gateway1, gateway2, gateway3);
        }

        [Test]
        public void can_build_with_ilist_dependency()
        {
            var build = new ConcreteBuild<GatewayIListUser>();

            build.ConstructorArgs(new AllPossibleValuesDependencySource(typeof (IList<IGateway>)));

            var arrayUser = build.Build<GatewayIListUser>(theSession);

            arrayUser.Gateways.ShouldHaveTheSameElementsAs(gateway1, gateway2, gateway3);
        }


        [Test]
        public void can_build_with_list_dependency()
        {
            var build = new ConcreteBuild<GatewayListUser>();

            build.ConstructorArgs(new AllPossibleValuesDependencySource(typeof (List<IGateway>)));

            var arrayUser = build.Build<GatewayListUser>(theSession);

            arrayUser.Gateways.ShouldHaveTheSameElementsAs(gateway1, gateway2, gateway3);
        }
    }
}