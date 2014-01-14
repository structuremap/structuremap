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
            var target = new Target();
            var inner = new LiteralPlan<Target>(target);

            var plan = new InterceptionPlan(typeof (ITarget), inner, new IInterceptor[] {new ActivatorInterceptor<ITarget>(x => x.Activate())});

            plan.ToBuilder<ITarget>()(new StubBuildSession())
                .ShouldBeTheSameAs(target);

            target.HasBeenActivated.ShouldBeTrue();
        }

        [Test]
        public void multiple_activators_taking_different_accept_types()
        {
            var target = new Target();
            var inner = new LiteralPlan<Target>(target);

            var plan = new InterceptionPlan(typeof(ITarget), inner, 
                new IInterceptor[]
                {
                    new ActivatorInterceptor<ITarget>(x => x.Activate()),
                    new ActivatorInterceptor<Target>(x => x.TurnGreen())
                });

            plan.ToBuilder<ITarget>()(new StubBuildSession())
                .ShouldBeTheSameAs(target);

            target.HasBeenActivated.ShouldBeTrue();
            target.Color.ShouldEqual("Green");
        }

        [Test]
        public void activator_that_fails_gets_wrapped_in_descriptive_text()
        {
            var target = new Target();
            var inner = new LiteralPlan<Target>(target);

            var interceptor = new ActivatorInterceptor<Target>(x => x.ThrowUp());
            var plan = new InterceptionPlan(typeof(ITarget), inner, 
                new IInterceptor[]
                {
                    interceptor
                });

            var ex = Exception<StructureMapInterceptorException>.ShouldBeThrownBy(() => {
                plan.ToBuilder<ITarget>()(new StubBuildSession());
            });

            ex.Title.ShouldContain(interceptor.Description);


        }
    }
}