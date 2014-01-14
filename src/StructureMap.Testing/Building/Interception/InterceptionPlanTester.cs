using System.Diagnostics;
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

        [Test]
        public void multiple_activators_taking_different_accept_types()
        {
            var target = new ActivatorTarget();
            var inner = new LiteralPlan<ActivatorTarget>(target);

            var plan = new InterceptionPlan(typeof(IActivatorTarget), inner, 
                new IInterceptor[]
                {
                    new ActivatorInterceptor<IActivatorTarget>(x => x.Activate()),
                    new ActivatorInterceptor<ActivatorTarget>(x => x.TurnGreen())
                });

            plan.ToBuilder<IActivatorTarget>()(new StubBuildSession())
                .ShouldBeTheSameAs(target);

            target.HasBeenActivated.ShouldBeTrue();
            target.Color.ShouldEqual("Green");
        }

        [Test]
        public void activator_that_fails_gets_wrapped_in_descriptive_text()
        {
            var target = new ActivatorTarget();
            var inner = new LiteralPlan<ActivatorTarget>(target);

            var interceptor = new ActivatorInterceptor<ActivatorTarget>(x => x.ThrowUp());
            var plan = new InterceptionPlan(typeof(IActivatorTarget), inner, 
                new IInterceptor[]
                {
                    interceptor
                });

            var ex = Exception<StructureMapInterceptorException>.ShouldBeThrownBy(() => {
                plan.ToBuilder<IActivatorTarget>()(new StubBuildSession());
            });

            ex.Title.ShouldContain(interceptor.Description);


        }
    }
}