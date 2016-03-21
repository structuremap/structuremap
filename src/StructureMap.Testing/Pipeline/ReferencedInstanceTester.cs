using Shouldly;
using StructureMap.Building;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class ReferencedInstanceTester
    {
        public interface IReferenced
        {
        }

        public class ConcreteReferenced : IReferenced
        {
        }

        [Fact]
        public void GetDescription()
        {
            var theReferenceKey = "theReferenceKey";
            var instance = new ReferencedInstance(theReferenceKey);

            instance.Description.ShouldBe("\"theReferenceKey\"");
        }

        [Fact]
        public void to_dependency_source()
        {
            var theReferenceKey = "theReferenceKey";
            var instance = new ReferencedInstance(theReferenceKey);

            instance.ToDependencySource(typeof(IGateway))
                .ShouldBe(new ReferencedDependencySource(typeof(IGateway), theReferenceKey));
        }
    }
}