using System;
using System.Diagnostics;
using System.Linq.Expressions;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Building;
using StructureMap.Building.Interception;

namespace StructureMap.Testing.Building.Interception
{
    [TestFixture]
    public class ActivatorInterceptorTester
    {
        private ActivatorInterceptor<IActivatorTarget> theActivator;

        [SetUp]
        public void SetUp()
        {
            theActivator = new ActivatorInterceptor<IActivatorTarget>(x => x.Activate());
        }

        [Test]
        public void the_description()
        {
            theActivator.Description.ShouldEqual("IActivatorTarget.Activate()");
        }

        [Test]
        public void the_role_is_activates()
        {
            theActivator.Role.ShouldEqual(InterceptorRole.Activates);
        }

        [Test]
        public void the_accepts_type()
        {
            theActivator.Accepts.ShouldEqual(typeof (IActivatorTarget));
        }
        [Test]
        public void the_return_type()
        {
            theActivator.Returns.ShouldEqual(typeof (IActivatorTarget));
        }

        [Test]
        public void create_the_expression_when_the_variable_is_the_right_type()
        {
            var variable = Expression.Variable(typeof (IActivatorTarget), "target");

            var expression = theActivator.ToExpression(Parameters.Session, variable);

            expression.ToString().ShouldEqual("target.Activate()");
        }

        [Test]
        public void compile_and_use_by_itself()
        {
            var variable = Expression.Variable(typeof(IActivatorTarget), "target");

            var expression = theActivator.ToExpression(Parameters.Session, variable);

            var lambdaType = typeof (Action<IActivatorTarget>);
            var lambda = Expression.Lambda(lambdaType, expression, variable);

            var action = lambda.Compile().As<Action<IActivatorTarget>>();

            var target = new ActivatorTarget();
            action(target);

            target.HasBeenActivated.ShouldBeTrue();
        }

    }

    public interface IActivatorTarget
    {
        void Activate();
    }

    public class ActivatorTarget : IActivatorTarget
    {
        public bool HasBeenActivated;

        void IActivatorTarget.Activate()
        {
            HasBeenActivated = true;
        }
    }
}