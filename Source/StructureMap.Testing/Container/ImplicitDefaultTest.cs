using NUnit.Framework;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Container
{
    [TestFixture]
    public class ImplicitDefaultTest
    {
        [Test]
        public void CanSetTheDefaultInstanceKeyImplicitly()
        {
            InstanceFactory factory = ObjectMother.CreateInstanceFactory(
                typeof (IGateway),
                new string[] {"StructureMap.Testing.Widget3"});

            DefaultGateway gateway = factory.GetInstance() as DefaultGateway;
            Assert.IsNotNull(gateway);
        }

        [Test]
        public void GetTheDefaultInstanceKeyFromType()
        {
            string default1 = PluginFamilyAttribute.GetDefaultKey(typeof (IGateway));
            string default2 = PluginFamilyAttribute.GetDefaultKey(typeof (IService));
            string default3 = PluginFamilyAttribute.GetDefaultKey(typeof (IWorker));

            Assert.AreEqual("Default", default1);
            Assert.AreEqual(string.Empty, default2);
            Assert.AreEqual(string.Empty, default3);
        }


        [Test]
        public void CanSetTheDefaultInstanceKeyImplicitlyFromObjectFactory()
        {
            DefaultGateway gateway = ObjectFactory.GetInstance(typeof (IGateway)) as DefaultGateway;
            Assert.IsNotNull(gateway);
        }
    }
}