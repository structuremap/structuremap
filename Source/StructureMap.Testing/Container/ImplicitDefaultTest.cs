using NUnit.Framework;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Container
{
    [TestFixture]
    public class ImplicitDefaultTest
    {
        [Test]
        public void CanSetTheDefaultInstanceKeyImplicitlyFromObjectFactory()
        {
            DefaultGateway gateway = ObjectFactory.GetInstance(typeof (IGateway)) as DefaultGateway;
            Assert.IsNotNull(gateway);
        }

    }
}