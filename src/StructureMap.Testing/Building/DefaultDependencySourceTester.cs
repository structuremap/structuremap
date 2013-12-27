using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Building
{
    [TestFixture]
    public class DefaultDependencySourceTester
    {
        [Test]
        public void can_resolve_through_build_session()
        {
            var session = new FakeBuildSession();
            var gateway = new StubbedGateway();

            session.SetDefault<IGateway>(gateway);

            var build = new ConcreteBuild<GuyWhoUsesGateway>();
            build.ConstructorArgs(new DefaultDependencySource(typeof (IGateway)));

            build.ToDelegate<GuyWhoUsesGateway>()(session)
                .Gateway.ShouldBeTheSameAs(gateway);
        }
    }

    [TestFixture]
    public class ReferencedDependencySourceTester
    {
        [Test]
        public void can_resolve_through_build_session()
        {
            var session = new FakeBuildSession();
            var gateway = new StubbedGateway();
            var gateway2 = new StubbedGateway();

            session.NamedObjects[typeof (IGateway)]["Red"] = gateway;
            session.NamedObjects[typeof (IGateway)]["Blue"] = gateway2;

            var build = new ConcreteBuild<GuyWhoUsesGateway>();
            build.ConstructorArgs(new ReferencedDependencySource(typeof(IGateway), "Blue"));

            build.ToDelegate<GuyWhoUsesGateway>()(session)
                .Gateway.ShouldBeTheSameAs(gateway2);
        }
    }

    public class GuyWhoUsesGateway
    {
        private readonly IGateway _gateway;

        public GuyWhoUsesGateway(IGateway gateway)
        {
            _gateway = gateway;
        }

        public IGateway Gateway
        {
            get { return _gateway; }
        }
    }
}