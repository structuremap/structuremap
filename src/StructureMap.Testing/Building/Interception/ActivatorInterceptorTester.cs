using System;
using System.Diagnostics;
using System.Linq.Expressions;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.Building;
using StructureMap.Building.Interception;

namespace StructureMap.Testing.Building.Interception
{
    [TestFixture]
    public class ActivatorInterceptorTester
    {
        private ActivatorInterceptor<ITarget> theActivator;

        [SetUp]
        public void SetUp()
        {
            theActivator = new ActivatorInterceptor<ITarget>(x => x.Activate());
        }

        [Test]
        public void the_description()
        {
            theActivator.Description.ShouldContain("ITarget.Activate()");
        }

        [Test]
        public void description_is_set_explicitly()
        {
            theActivator = new ActivatorInterceptor<ITarget>(x => x.Activate(), "gonna start it up");
            
            theActivator.Description.ShouldContain("gonna start it up");
        }

        [Test]
        public void the_description_using_session()
        {
            var activator = new ActivatorInterceptor<Target>((s, t) => t.UseSession(s));

            activator.Description.ShouldContain("Target.UseSession(IContext)");
        }

        [Test]
        public void the_description_using_session_and_explicit_description()
        {
            var activator = new ActivatorInterceptor<Target>((s, t) => t.UseSession(s), "use the Force Luke!");

            activator.Description.ShouldContain("use the Force Luke!");
        }

        [Test]
        public void the_role_is_activates()
        {
            theActivator.Role.ShouldBe(InterceptorRole.Activates);
        }

        [Test]
        public void the_accepts_type()
        {
            theActivator.Accepts.ShouldBe(typeof (ITarget));
        }
        [Test]
        public void the_return_type()
        {
            theActivator.Returns.ShouldBe(typeof (ITarget));
        }

        [Test]
        public void create_the_expression_when_the_variable_is_the_right_type()
        {
            var variable = Expression.Variable(typeof (ITarget), "target");

            var expression = theActivator.ToExpression(new Policies(), Parameters.Session, variable);

            expression.ToString().ShouldBe("target.Activate()");
        }

        [Test]
        public void compile_and_use_by_itself()
        {
            var variable = Expression.Variable(typeof(ITarget), "target");

            var expression = theActivator.ToExpression(new Policies(), Parameters.Context, variable);

            var lambdaType = typeof (Action<ITarget>);
            var lambda = Expression.Lambda(lambdaType, expression, variable);

            var action = lambda.Compile().As<Action<ITarget>>();

            var target = new Target();
            action(target);

            target.HasBeenActivated.ShouldBeTrue();
        }

        [Test]
        public void compile_and_use_by_itself_with_session()
        {
            var activator = new ActivatorInterceptor<Target>((s, t) => t.UseSession(s));
            var variable = Expression.Variable(typeof(Target), "target");



            var expression = activator.ToExpression(new Policies(), Parameters.Context, variable);

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