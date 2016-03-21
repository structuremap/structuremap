using Shouldly;
using StructureMap.Building;
using StructureMap.Building.Interception;
using System;
using System.Linq.Expressions;
using Xunit;

namespace StructureMap.Testing.Building.Interception
{
    public class ActivatorInterceptorTester
    {
        private ActivatorInterceptor<ITarget> theActivator;

        public ActivatorInterceptorTester()
        {
            theActivator = new ActivatorInterceptor<ITarget>(x => x.Activate());
        }

        [Fact]
        public void the_description()
        {
            theActivator.Description.ShouldContain("ITarget.Activate()");
        }

        [Fact]
        public void description_is_set_explicitly()
        {
            theActivator = new ActivatorInterceptor<ITarget>(x => x.Activate(), "gonna start it up");

            theActivator.Description.ShouldContain("gonna start it up");
        }

        [Fact]
        public void the_description_using_session()
        {
            var activator = new ActivatorInterceptor<Target>((s, t) => t.UseSession(s));

            activator.Description.ShouldContain("Target.UseSession(IContext)");
        }

        [Fact]
        public void the_description_using_session_and_explicit_description()
        {
            var activator = new ActivatorInterceptor<Target>((s, t) => t.UseSession(s), "use the Force Luke!");

            activator.Description.ShouldContain("use the Force Luke!");
        }

        [Fact]
        public void the_role_is_activates()
        {
            theActivator.Role.ShouldBe(InterceptorRole.Activates);
        }

        [Fact]
        public void the_accepts_type()
        {
            theActivator.Accepts.ShouldBe(typeof(ITarget));
        }

        [Fact]
        public void the_return_type()
        {
            theActivator.Returns.ShouldBe(typeof(ITarget));
        }

        [Fact]
        public void create_the_expression_when_the_variable_is_the_right_type()
        {
            var variable = Expression.Variable(typeof(ITarget), "target");

            var expression = theActivator.ToExpression(Policies.Default(), Parameters.Session, variable);

            expression.ToString().ShouldBe("target.Activate()");
        }

        [Fact]
        public void compile_and_use_by_itself()
        {
            var variable = Expression.Variable(typeof(ITarget), "target");

            var expression = theActivator.ToExpression(Policies.Default(), Parameters.Context, variable);

            var lambdaType = typeof(Action<ITarget>);
            var lambda = Expression.Lambda(lambdaType, expression, variable);

            var action = lambda.Compile().As<Action<ITarget>>();

            var target = new Target();
            action(target);

            target.HasBeenActivated.ShouldBeTrue();
        }

        [Fact]
        public void compile_and_use_by_itself_with_session()
        {
            var activator = new ActivatorInterceptor<Target>((s, t) => t.UseSession(s));
            var variable = Expression.Variable(typeof(Target), "target");

            var expression = activator.ToExpression(Policies.Default(), Parameters.Context, variable);

            var lambdaType = typeof(Action<IContext, Target>);
            var lambda = Expression.Lambda(lambdaType, expression, Parameters.Context, variable);

            var action = lambda.Compile().As<Action<IContext, Target>>();

            var target = new Target();
            var session = new FakeBuildSession();
            action(session, target);

            target.Session.ShouldBeTheSameAs(session);
        }
    }

    public interface ITarget
    {
        void Activate();

        void Debug();
    }

    public class Target : ITarget
    {
        public bool HasBeenActivated;

        void ITarget.Activate()
        {
            HasBeenActivated = true;
        }

        public void Debug()
        {
            throw new NotImplementedException();
        }

        public void TurnGreen()
        {
            Color = "Green";
        }

        public void UseSession(IContext session)
        {
            Session = session;
        }

        public IContext Session;

        public string Color = "Red";

        public void ThrowUp()
        {
            throw new DivideByZeroException("you stink!");
        }

        public void BlowUpOnSession(IContext session)
        {
            throw new NotImplementedException();
        }
    }
}