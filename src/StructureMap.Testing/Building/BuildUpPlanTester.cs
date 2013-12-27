using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Building
{
    [TestFixture]
    public class BuildUpPlanTester
    {
        [Test]
        public void can_build_setters_on_an_existing_object()
        {
            var target = new SetterTarget();

            var gateway = new StubbedGateway();
            var session = new FakeBuildSession();
            session.SetDefault<IGateway>(gateway);

            var plan = new BuildUpPlan<SetterTarget>();
            plan.Set(x => x.Color, "Red");
            plan.Set(x => x.Direction, "Green");
            plan.Set(x => x.Gateway, new DefaultDependencySource(typeof(IGateway)));

            plan.BuildUp(session, target);

            target.Color.ShouldEqual("Red");
            target.Direction.ShouldEqual("Green");
            target.Gateway.ShouldBeTheSameAs(gateway);
        }
    }

}