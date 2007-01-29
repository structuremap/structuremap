using System.Reflection;
using NUnit.Framework;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Container
{
    [TestFixture]
    public class ContainerConstructorAttributeTester
    {
        public ContainerConstructorAttributeTester()
        {
        }

        [Test]
        public void GetConstructor()
        {
            ConstructorInfo constructor = DefaultConstructorAttribute.GetConstructor(
                typeof (ComplexRule));

            Assert.IsNotNull(constructor);
        }
    }
}