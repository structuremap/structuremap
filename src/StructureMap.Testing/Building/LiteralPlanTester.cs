using System;
using System.Linq.Expressions;
using NUnit.Framework;
using StructureMap.Building;

namespace StructureMap.Testing.Building
{
    [TestFixture]
    public class LiteralPlanTester
    {
        [Test]
        public void just_returns_the_object()
        {
            var foo = new Foo(Guid.NewGuid());
            var plan = new LiteralPlan<Foo>(foo, "some foo");

            plan.Build<Foo>(new FakeBuildSession())
                .ShouldBeTheSameAs(foo);
        }

        [Test]
        public void get_the_description_when_it_is_explicitly_set()
        {
            var foo = new Foo(Guid.NewGuid());
            var plan = new LiteralPlan<Foo>(foo, "some foo");

            plan.Description.ShouldEqual("some foo");
        }

        [Test]
        public void get_the_description_when_the_object_is_null()
        {
            var plan = new LiteralPlan<Foo>(null);
            plan.Description.ShouldEqual("null");
        }

        [Test]
        public void derive_the_description_from_the_object()
        {
            var foo = new Foo(Guid.NewGuid());
            var plan = new LiteralPlan<Foo>(foo);

            plan.Description.ShouldEqual(foo.ToString());
        }

        [Test]
        public void return_the_expression()
        {
            var foo = new Foo(Guid.NewGuid());
            var plan = new LiteralPlan<Foo>(foo);

            var expression = plan.ToExpression(null)
                .ShouldBeOfType<ConstantExpression>();

            expression.Value.ShouldEqual(foo);
            expression.Type.ShouldEqual(typeof (Foo));
        }
    }
}