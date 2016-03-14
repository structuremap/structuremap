using StructureMap.Testing.Widget;
using Xunit;

namespace StructureMap.Testing.Graph
{
    public class ContainerConstructorAttributeTester
    {
        [Fact]
        public void GetConstructor()
        {
            var constructor = DefaultConstructorAttribute.GetConstructor(
                typeof(ComplexRule));

            constructor.ShouldNotBeNull();
        }
    }
}