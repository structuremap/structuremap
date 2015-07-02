using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Shouldly;
using StructureMap.Building;
using StructureMap.Building.Interception;

namespace StructureMap.Testing.Building.Interception
{
    [TestFixture]
    public class FuncInterceptorTester
    {
        private FuncInterceptor<ITarget> theInterceptor;

        [SetUp]
        public void SetUp()
        {
            theInterceptor = new FuncInterceptor<ITarget>(x => new DecoratedTarget(x));
        }

        [Test]
        public void role_is_decorator()
        {
            theInterceptor.Role.ShouldBe(InterceptorRole.Decorates);
        }

        [Test]
        public void accepts_type()
        {
            theInterceptor.Accepts.ShouldBe(typeof (ITarget));
        }

        [Test]
        public void return_type()
        {
            theInterceptor.Returns.ShouldBe(typeof (ITarget));
        }

        [Test]
        public void description_comes_from_the_body()
        {
            theInterceptor.Description.ShouldContain("new DecoratedTarget(ITarget)");
        }

        [Test]
        public void explicit_description()
        {
            theInterceptor = new FuncInterceptor<ITarget>(x => new DecoratedTarget(x), "decorating the target");

            theInterceptor.Description.ShouldContain("decorating the target");
        }

        [Test]
        public void description_when_uses_IContext_too()
        {
            theInterceptor = new FuncInterceptor<ITarget>((c, t) => new ContextKeepingTarget(c, t));

            theInterceptor.Description.ShouldContain("new ContextKeepingTarget(IContext, ITarget)");
        }

        [Test]
        public void explicit_description_with_icontext()
        {
            theInterceptor = new FuncInterceptor<ITarget>((c, t) => new ContextKeepingTarget(c, t), "context keeping");

            theInterceptor.Description.ShouldContain("context keeping");
        }

        [Test]
        public void compile_and_use_by_itself_not_using_IBuildSession()
        {
            var variable = Expression.Variable(typeof (ITarget), "target");

            var expression = theInterceptor.ToExpression(new Policies(), Parameters.Session, variable);

            var lambdaType = typeof (Func<ITarget, ITarget>);
            var lambda = Expression.Lambda(lambdaType, expression, variable);

            var func = lambda.Compile().As<Func<ITarget, ITarget>>();

            var target = new Target();
            var decorated = func(target);

            decorated.ShouldBeOfType<DecoratedTarget>()
                .Inner.ShouldBeTheSameAs(target);
        }


        [Test]
        public void compile_and_use_by_itself_using_IContext()
        {
            theInterceptor = new FuncInterceptor<ITarget>((c, t) => new ContextKeepingTarget(c, t));

            var variable = Expression.Variable(typeof (ITarget), "target");

            var expression = theInterceptor.ToExpression(new Policies(), Parameters.Context, variable);

            var lambdaType = typeof (Func<IContext, ITarget, ITarget>);
            var lambda = Expression.Lambda(lambdaType, expression, Parameters.Context, variable);

            var func = lambda.Compile().As<Func<IContext, ITarget, ITarget>>();

            var target = new Target();
            var session = new FakeBuildSession();
            var decorated = func(session, target).ShouldBeOfType<ContextKeepingTarget>();

            decorated
                .Inner.ShouldBeTheSameAs(target);

            decorated.Session.ShouldBeTheSameAs(session);
        }
    }

    public class DecoratedTarget : ITarget
    {
        private readonly ITarget _inner;

        public DecoratedTarget(ITarget inner)
        {
            _inner = inner;
        }

        public ITarget Inner
        {
            get { return _inner; }
        }

        public void Activate()
        {
        }

        public void Debug()
        {
            throw new NotImplementedException();
        }
    }

    public class BorderedTarget : DecoratedTarget
    {
        public BorderedTarget(ITarget inner) : base(inner)
        {
        }
    }

    public class ThrowsDecoratedTarget : ITarget
    {
        public ThrowsDecoratedTarget(ITarget inner)
        {
            throw new DivideByZeroException("you failed!");
        }


        public void Activate()
        {
        }

        public void Debug()
        {
            throw new NotImplementedException();
        }
    }

    public class ContextKeepingTarget : ITarget
    {
        private readonly IContext _session;
        private readonly ITarget _inner;

        public ContextKeepingTarget(IContext session, ITarget inner)
        {
            _session = session;
            _inner = inner;
        }

        public IContext Session
        {
            get { return _session; }
        }

        public ITarget Inner
        {
            get { return _inner; }
        }

        public void Activate()
        {
            throw new NotImplementedException();
        }

        public void Debug()
        {
            throw new NotImplementedException();
        }
    }

    public class SadContextKeepingTarget : ContextKeepingTarget
    {
        public SadContextKeepingTarget(IContext session, ITarget inner) : base(session, inner)
        {
            throw new DivideByZeroException("no soup for you!");
        }
    }
}