using StructureMap.Pipeline;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class GreediestConstructorSelectorTester
    {
        [Fact]
        public void has_missing_primitives_positive()
        {
            var ctor = typeof(GuyWithPrimitiveArgs).GetConstructors().Single();

            var dependencies = new DependencyCollection();

            GreediestConstructorSelector.HasMissingPrimitives(ctor, dependencies).ShouldBeTrue();

            dependencies.Add("name", "Jeremy");

            // One is still missing
            GreediestConstructorSelector.HasMissingPrimitives(ctor, dependencies).ShouldBeTrue();
        }

        [Fact]
        public void has_missing_primitives_positive_2()
        {
            var ctor = typeof(GuyWithPrimitiveArgs).GetConstructors().Single();

            var dependencies = new DependencyCollection();

            dependencies.Add("age", 1);

            // One is still missing
            GreediestConstructorSelector.HasMissingPrimitives(ctor, dependencies).ShouldBeTrue();
        }

        [Fact]
        public void has_missing_primitives_negative()
        {
            var ctor = typeof(GuyWithPrimitiveArgs).GetConstructors().Single();

            var dependencies = new DependencyCollection();

            dependencies.Add("name", "Jeremy");
            dependencies.Add(typeof(int), 41);

            // One is still missing
            GreediestConstructorSelector.HasMissingPrimitives(ctor, dependencies).ShouldBeFalse();
        }
    }

    public class GuyWithPrimitiveArgs
    {
        public GuyWithPrimitiveArgs(string name, int age)
        {
        }
    }
}