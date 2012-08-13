using NUnit.Framework;
using StructureMap.Testing.Configuration;

namespace StructureMap.Testing
{
    [TestFixture]
    public class PrimitiveArrayTester
    {
        [Test]
        public void specify_a_string_array()
        {
            var container = new Container(x =>
            {
                x.ForConcreteType<ClassWithStringAndIntArray>().Configure
                    .CtorDependency<string[]>().Is(new[] {"a", "b", "c"})
                    .CtorDependency<int[]>().Is(new[] {1, 2, 3});
            });

            var objectWithArrays = container.GetInstance<ClassWithStringAndIntArray>();
            objectWithArrays.Numbers.ShouldEqual(new[] {1, 2, 3});
            objectWithArrays.Strings.ShouldEqual(new[] {"a", "b", "c"});
        }
    }
}