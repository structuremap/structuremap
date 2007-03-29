using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing
{
    [TestFixture]
    public class GenericsIntegrationTester
    {
        private InstanceManager manager;

        [SetUp]
        public void SetUp()
        {
            PluginGraph graph = DataMother.GetDiagnosticPluginGraph("GenericsTesting.xml");
            manager = new InstanceManager(graph);
        }

        [Test]
        public void SimpleInstanceManagerTestWithGenerics()
        {
            Service<int> intService = (Service<int>) manager.CreateInstance(typeof (IService<int>), "Default");
            Assert.AreEqual(typeof (int), intService.GetT());

            Service<string> stringService =
                (Service<string>) manager.CreateInstance(typeof (IService<string>), "Default");
            Assert.AreEqual(typeof (string), stringService.GetT());
        }

        [Test]
        public void MultipleGenericTypes()
        {
            IService<int> intService = (IService<int>) manager.CreateInstance(typeof (IService<int>), "Default");
            IService<string> stringService =
                (IService<string>) manager.CreateInstance(typeof (IService<string>), "Default");
            IService<double> doubleService =
                (IService<double>) manager.CreateInstance(typeof (IService<double>), "Default");
        }

        [Test]
        public void PicksUpASimpleGenericPluginFamilyFromConfiguration()
        {
            ISimpleThing<int> thing = (ISimpleThing<int>) manager.CreateInstance(typeof (ISimpleThing<int>));
            Assert.IsNotNull(thing);
        }

        [Test, Ignore("Generics with more than 2 parameters")]
        public void ImplicitPluginFamilyWithLotsOfTemplatedParameters()
        {
            ILotsOfTemplatedTypes<int, bool, string> thing =
                (ILotsOfTemplatedTypes<int, bool, string>)
                manager.CreateInstance(typeof (ILotsOfTemplatedTypes<int, bool, string>));

            Assert.IsNotNull(thing);
        }

        [Test]
        public void PicksUpAnExplicitlyDefinedGenericPluginFamilyFromConfiguration()
        {
            IThing<int, string> thing =
                (IThing<int, string>) manager.CreateInstance(typeof (IThing<int, string>));
            ColorThing<int, string> redThing = (ColorThing<int, string>) thing;

            Assert.AreEqual("Red", redThing.Color);
        }

        [Test]
        public void SingletonInterceptors()
        {
            AbstractClass<int> object1 = (AbstractClass<int>) manager.CreateInstance(typeof (AbstractClass<int>));
            AbstractClass<int> object2 = (AbstractClass<int>) manager.CreateInstance(typeof (AbstractClass<int>));
            AbstractClass<int> object3 = (AbstractClass<int>) manager.CreateInstance(typeof (AbstractClass<int>));

            AbstractClass<string> object4 =
                (AbstractClass<string>) manager.CreateInstance(typeof (AbstractClass<string>));
            AbstractClass<string> object5 =
                (AbstractClass<string>) manager.CreateInstance(typeof (AbstractClass<string>));
            AbstractClass<string> object6 =
                (AbstractClass<string>) manager.CreateInstance(typeof (AbstractClass<string>));

            Assert.AreSame(object1, object2);
            Assert.AreSame(object1, object3);
            Assert.AreSame(object4, object5);
            Assert.AreSame(object4, object6);

            Assert.AreNotSame(object1, object4);
        }
    }
}