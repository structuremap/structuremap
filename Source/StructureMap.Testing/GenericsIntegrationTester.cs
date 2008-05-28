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
            string xml = @"
<StructureMap Id='Generics'>
  <Assembly Name='StructureMap.Testing.GenericWidgets'/>

  <PluginFamily Assembly='StructureMap.Testing.GenericWidgets' Type='StructureMap.Testing.GenericWidgets.IThing`2' DefaultKey='Red'>
    <Plugin Assembly='StructureMap.Testing.GenericWidgets' Type='StructureMap.Testing.GenericWidgets.ColorThing`2' ConcreteKey='Color' />
    <Plugin Assembly='StructureMap.Testing.GenericWidgets' Type='StructureMap.Testing.GenericWidgets.ComplexThing`2' ConcreteKey='Complex' />

    <Instance Key='Red' Type='Color'>
      <Property Name='color' Value='Red'/>
    </Instance>

    <Instance Key='Complicated' Type='Complex'>
      <Property Name='name' Value='Jeremy' />
      <Property Name='age' Value='32' />
      <Property Name='ready' Value='true' />
    </Instance>
  </PluginFamily>

  <PluginFamily Assembly='StructureMap.Testing.GenericWidgets' Type='StructureMap.Testing.GenericWidgets.ISimpleThing`1' DefaultKey='Simple'>
    <Plugin Assembly='StructureMap.Testing.GenericWidgets' Type='StructureMap.Testing.GenericWidgets.SimpleThing`1' ConcreteKey='Simple' />
  </PluginFamily>
  
</StructureMap>
";

            PluginGraph graph = DataMother.BuildPluginGraphFromXml(xml);
            manager = new StructureMap.Container(graph);
        }

        #endregion

        private StructureMap.Container manager;

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


        [Test]
        public void MultipleGenericTypes()
        {
            IService<int> intService = (IService<int>) manager.GetInstance(typeof (IService<int>), "Default");
            IService<string> stringService =
                (IService<string>) manager.GetInstance(typeof (IService<string>), "Default");
            IService<double> doubleService =
                (IService<double>) manager.GetInstance(typeof (IService<double>), "Default");
        }

        [Test]
        public void PicksUpAnExplicitlyDefinedGenericPluginFamilyFromConfiguration()
        {
            IThing<int, string> thing =
                (IThing<int, string>) manager.GetInstance(typeof (IThing<int, string>));
            ColorThing<int, string> redThing = (ColorThing<int, string>) thing;

            Assert.AreEqual("Red", redThing.Color);
        }

        [Test]
        public void PicksUpASimpleGenericPluginFamilyFromConfiguration()
        {
            ISimpleThing<int> thing = (ISimpleThing<int>) manager.GetInstance(typeof (ISimpleThing<int>));
            Assert.IsNotNull(thing);
        }

        [Test]
        public void SimpleInstanceManagerTestWithGenerics()
        {
            Service<int> intService = (Service<int>) manager.GetInstance(typeof (IService<int>), "Default");
            Assert.AreEqual(typeof (int), intService.GetT());

            Service<string> stringService =
                (Service<string>) manager.GetInstance(typeof (IService<string>), "Default");
            Assert.AreEqual(typeof (string), stringService.GetT());
        }

        [Test]
        public void SingletonInterceptors()
        {
            AbstractClass<int> object1 = (AbstractClass<int>) manager.GetInstance(typeof (AbstractClass<int>));
            AbstractClass<int> object2 = (AbstractClass<int>) manager.GetInstance(typeof (AbstractClass<int>));
            AbstractClass<int> object3 = (AbstractClass<int>) manager.GetInstance(typeof (AbstractClass<int>));

            AbstractClass<string> object4 =
                (AbstractClass<string>) manager.GetInstance(typeof (AbstractClass<string>));
            AbstractClass<string> object5 =
                (AbstractClass<string>) manager.GetInstance(typeof (AbstractClass<string>));
            AbstractClass<string> object6 =
                (AbstractClass<string>) manager.GetInstance(typeof (AbstractClass<string>));

            Assert.AreSame(object1, object2);
            Assert.AreSame(object1, object3);
            Assert.AreSame(object4, object5);
            Assert.AreSame(object4, object6);

            Assert.AreNotSame(object1, object4);
        }

        [Test]
        public void SpecificImplementation()
        {
            IConcept<object> concept = (IConcept<object>) manager.GetInstance(typeof (IConcept<object>), "Specific");

            Assert.IsNotNull(concept);
            Assert.IsInstanceOfType(typeof (SpecificConcept), concept);
        }
    }
}