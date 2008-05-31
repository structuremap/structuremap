using System.Reflection;
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
            ConstructorInfo constructor = DefaultConstructorAttribute.GetConstructor(
                typeof (ComplexRule));

            Assert.IsNotNull(constructor);
        }
    }
}