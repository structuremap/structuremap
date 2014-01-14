using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Building.Interception;
using StructureMap.Testing.Pipeline;

namespace StructureMap.Testing.Building.Interception
{
    [TestFixture]
    public class InterceptionPlanTester
    {
        [Test]
        public void intercept_happy_path_with_a_single_activation()
        {
            var target = new ActivatorTarget();
            var inner = new LiteralPlan<ActivatorTarget>(target);

            var plan = new InterceptionPlan(typeof (IActivatorTarget), inner, new IInterceptor[] {new ActivatorInterceptor<IActivatorTarget>(x => x.Activate())});

            plan.ToBuilder<IActivatorTarget>()(new StubBuildSession())
                .ShouldBeTheSameAs(target);

            target.HasBeenActivated.ShouldBeTrue();
        }
    }
}