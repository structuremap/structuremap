using System;
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

            plan.ToDelegate<Foo>()(new FakeContext())
                .ShouldBeTheSameAs(foo);
        }
    }
}