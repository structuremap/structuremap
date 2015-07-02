using System;
using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class ClassWithoutPublicConstructorNullReferenceException
    {
        [Test]
        public void should_not_throw_a_null_reference_exception()
        {
            var container =
                new Container(
                    x =>
                    {
                        Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => { x.For<Test>().Use<Test>(); });
                    });
        }

        private class Test
        {
            protected Test()
            {
            }
        }
    }
}