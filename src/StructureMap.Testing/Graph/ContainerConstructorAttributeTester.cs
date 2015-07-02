using NUnit.Framework;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class ContainerConstructorAttributeTester
    {
        [Test]
        public void GetConstructor()
        {
            var constructor = DefaultConstructorAttribute.GetConstructor(
                typeof (ComplexRule));

            constructor.IsNotNull();
        }
    }
}