using System.Collections;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing
{
    [TestFixture]
    public class GenericsIntegrationTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            PluginGraph graph = DataMother.GetDiagnosticPluginGraph("GenericsTesting.xml");
            manager = new InstanceManager(graph);
        }

        #endregion

        private InstanceManager manager;

        [Test]
        public void AllTypesWithSpecificImplementation()
        {
            IList objectConcepts = manager.GetAllInstances(typeof (IConcept<object>));

            Assert.IsNotNull(objectConcepts);
            Assert.AreEqual(2, objectConcepts.Count);

            IList stringConcepts = manager.GetAllInstances(typeof (IConcept<string>));

            Assert.IsNotNull(stringConcepts);
            Assert.AreEqual(1, stringConcepts.Count);
        }

        [Test]
        public void Plugin_can_service_a_generic_type()
        {
            Assert.IsTrue(GenericsPluginGraph.CanBePluggedIntoGenericType(typeof(IConcept<>), typeof(SpecificConcept), typeof(object)));
            Assert.IsFalse(GenericsPluginGraph.CanBePluggedIntoGenericType(typeof(IConcept<>), typeof(SpecificConcept), typeof(string)));
            Assert.IsFalse(GenericsPluginGraph.CanBePluggedIntoGenericType(typeof(IConcept<>), typeof(SpecificConcept), typeof(int)));
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
        public void MultipleGenericTypes()
        {
            IService<int> intService = (IService<int>) manager.CreateInstance(typeof (IService<int>), "Default");
            IService<string> stringService =
                (IService<string>) manager.CreateInstance(typeof (IService<string>), "Default");
            IService<double> doubleService =
                (IService<double>) manager.CreateInstance(typeof (IService<double>), "Default");
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
        public void PicksUpASimpleGenericPluginFamilyFromConfiguration()
        {
            ISimpleThing<int> thing = (ISimpleThing<int>) manager.CreateInstance(typeof (ISimpleThing<int>));
            Assert.IsNotNull(thing);
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

        [Test]
        public void SpecificImplementation()
        {
            IConcept<object> concept = (IConcept<object>) manager.CreateInstance(typeof (IConcept<object>), "Specific");

            Assert.IsNotNull(concept);
            Assert.IsInstanceOfType(typeof (SpecificConcept), concept);
        }
    }
}