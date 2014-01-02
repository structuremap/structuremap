using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Building
{
    [TestFixture]
    public class LifecycleDependencySourceTester
    {
        [Test]
        public void can_use_lifecyle_resolver_for_dependency()
        {
            var build = new ConcreteBuild<LifecycleTarget>();
            var gateway = new StubbedGateway();
            var instance = new ObjectInstance(gateway);

            var session = new FakeBuildSession();
            session.LifecycledObjects[typeof (IGateway)][instance]
                = gateway;

            var arg = new LifecycleDependencySource(typeof (IGateway), instance);
            build.ConstructorArgs(arg);

            var target = build.Build<LifecycleTarget>(session);
            target.Gateway.ShouldBeTheSameAs(gateway);
        }
    }

    public class LifecycleTarget
    {
        private readonly IGateway _gateway;

        public LifecycleTarget(IGateway gateway)
        {
            _gateway = gateway;
        }

        public IGateway Gateway
        {
            get { return _gateway; }
        }
    }
}