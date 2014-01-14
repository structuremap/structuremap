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

            var plan = new InterceptionPlan(typeof (ITarget), inner,
                new IInterceptor[]
                {
                    new ActivatorInterceptor<ITarget>(x => x.Activate())
                });

            plan.ToBuilder<ITarget>()(new StubBuildSession())
                .ShouldBeTheSameAs(target);

            target.HasBeenActivated.ShouldBeTrue();
        }

        [Test]
        public void intercept_happy_path_with_a_single_activation_that_uses_session()
        {
            var target = new Target();
            var inner = new LiteralPlan<Target>(target);

            var plan = new InterceptionPlan(typeof (ITarget), inner,
                new IInterceptor[] {new ActivatorInterceptor<Target>((session, x) => x.UseSession(session))});

            var theSession = new StubBuildSession();
            plan.ToBuilder<ITarget>()(theSession)
                .ShouldBeTheSameAs(target);

            target.Session.ShouldBeTheSameAs(theSession);
        }

        [Test]
        public void intercept_sad_path_with_a_single_activation_that_uses_session()
        {
            var target = new Target();
            var inner = new LiteralPlan<Target>(target);

            var plan = new InterceptionPlan(typeof (ITarget), inner,
                new IInterceptor[] {new ActivatorInterceptor<Target>((session, x) => x.BlowUpOnSession(session))});

            var theSession = new StubBuildSession();

            var ex =
                Exception<StructureMapInterceptorException>.ShouldBeThrownBy(
                    () => { plan.ToBuilder<ITarget>()(theSession); });

            ex.Message.ShouldContain("Target.BlowUpOnSession(IBuildSession)");
        }

        [Test]
        public void multiple_activators_taking_different_accept_types()
        {
            var target = new Target();
            var inner = new LiteralPlan<Target>(target);

            var plan = new InterceptionPlan(typeof (ITarget), inner,
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
            var plan = new InterceptionPlan(typeof (ITarget), inner,
                new IInterceptor[]
                {
                    interceptor
                });

            var ex =
                Exception<StructureMapInterceptorException>.ShouldBeThrownBy(
                    () => { plan.ToBuilder<ITarget>()(new StubBuildSession()); });

            ex.Title.ShouldContain(interceptor.Description);
        }

        [Test]
        public void single_decorator_happy_path_that_uses_the_plugin_type()
        {
            var target = new Target();
            var inner = new LiteralPlan<Target>(target);

            var decorator = new FuncInterceptor<ITarget>(t => new DecoratedTarget(t));
            var plan = new InterceptionPlan(typeof (ITarget), inner, new IInterceptor[] {decorator});

            plan.ToBuilder<ITarget>()(new StubBuildSession())
                .ShouldBeOfType<DecoratedTarget>()
                .Inner.ShouldBeTheSameAs(target);
        }

        [Test]
        public void multiple_decorators_happy_path()
        {
            var target = new Target();
            var inner = new LiteralPlan<Target>(target);

            var decorator1 = new FuncInterceptor<ITarget>(t => new DecoratedTarget(t));
            var decorator2 = new FuncInterceptor<ITarget>(t => new BorderedTarget(t));
            var plan = new InterceptionPlan(typeof (ITarget), inner, new IInterceptor[] {decorator1, decorator2});

            plan.ToBuilder<ITarget>()(new StubBuildSession())
                .ShouldBeOfType<BorderedTarget>()
                .Inner.ShouldBeOfType<DecoratedTarget>()
                .Inner.ShouldBeTheSameAs(target);
        }

        [Test]
        public void mixed_activators_and_decorators_happy_path()
        {
            var target = new Target();
            var inner = new LiteralPlan<Target>(target);

            var decorator1 = new FuncInterceptor<ITarget>(t => new DecoratedTarget(t));
            var decorator2 = new FuncInterceptor<ITarget>(t => new BorderedTarget(t));
            var plan = new InterceptionPlan(typeof (ITarget), inner, new IInterceptor[]
            {
                decorator1, 
                decorator2, 
                new ActivatorInterceptor<ITarget>(x => x.Activate()),
                new ActivatorInterceptor<Target>(x => x.TurnGreen())
            });

            plan.ToBuilder<ITarget>()(new StubBuildSession())
                .ShouldBeOfType<BorderedTarget>()
                .Inner.ShouldBeOfType<DecoratedTarget>()
                .Inner.ShouldBeTheSameAs(target);

            target.Color.ShouldEqual("Green");
            target.HasBeenActivated.ShouldBeTrue();
        }

        [Test]
        public void single_decorator_sad_path_that_uses_the_plugin_type()
        {
            var target = new Target();
            var inner = new LiteralPlan<Target>(target);

            var decorator = new FuncInterceptor<ITarget>(t => new ThrowsDecoratedTarget(t));
            var plan = new InterceptionPlan(typeof (ITarget), inner, new IInterceptor[] {decorator});

            var ex =
                Exception<StructureMapInterceptorException>.ShouldBeThrownBy(
                    () => { plan.ToBuilder<ITarget>()(new StubBuildSession()); });

            ex.Message.ShouldContain("new ThrowsDecoratedTarget(ITarget)");
        }


        [Test]
        public void single_decorator_happy_path_that_uses_the_plugin_type_and_session()
        {
            var target = new Target();
            var inner = new LiteralPlan<Target>(target);

            var decorator = new FuncInterceptor<ITarget>((s, t) => new ContextKeepingTarget(s, t));
            var plan = new InterceptionPlan(typeof (ITarget), inner, new IInterceptor[] {decorator});

            var theSession = new StubBuildSession();
            var result = plan.ToBuilder<ITarget>()(theSession)
                .ShouldBeOfType<ContextKeepingTarget>();

            result.Inner.ShouldBeTheSameAs(target);
            result.Session.ShouldBeTheSameAs(theSession);
        }

        [Test]
        public void single_decorator_sad_path_that_uses_the_plugin_type_and_session()
        {
            var target = new Target();
            var inner = new LiteralPlan<Target>(target);

            var decorator = new FuncInterceptor<ITarget>((s, t) => new SadContextKeepingTarget(s, t));
            var plan = new InterceptionPlan(typeof (ITarget), inner, new IInterceptor[] {decorator});

            var theSession = new StubBuildSession();


            var ex =
                Exception<StructureMapInterceptorException>.ShouldBeThrownBy(
                    () => { plan.ToBuilder<ITarget>()(theSession); });

            ex.Message.ShouldContain("new SadContextKeepingTarget(IBuildSession, ITarget)");
        }
    }
}