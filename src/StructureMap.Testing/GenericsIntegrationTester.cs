using System;
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
            string xml =
                @"
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
            manager = new Container(graph);
        }

        #endregion

        private Container manager;

        [Test, Ignore("not sure I want this behavior anyway")]
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
        public void MultipleGenericTypes()
        {
            var intService = (IService<int>) manager.GetInstance(typeof (IService<int>), "Default");
            var stringService =
                (IService<string>) manager.GetInstance(typeof (IService<string>), "Default");
            var doubleService =
                (IService<double>) manager.GetInstance(typeof (IService<double>), "Default");
        }


        [Test]
        public void PicksUpAnExplicitlyDefinedGenericPluginFamilyFromConfiguration()
        {
            var thing =
                (IThing<int, string>) manager.GetInstance(typeof (IThing<int, string>));
            var redThing = (ColorThing<int, string>) thing;

            Assert.AreEqual("Red", redThing.Color);
        }

        [Test]
        public void PicksUpASimpleGenericPluginFamilyFromConfiguration()
        {
            var thing = (ISimpleThing<int>) manager.GetInstance(typeof (ISimpleThing<int>));
            Assert.IsNotNull(thing);
        }

        [Test]
        public void Plugin_can_service_a_generic_type()
        {
            Assert.IsTrue(GenericsPluginGraph.CanBePluggedIntoGenericType(typeof (IConcept<>), typeof (SpecificConcept),
                                                                          typeof (object)));
            Assert.IsFalse(GenericsPluginGraph.CanBePluggedIntoGenericType(typeof (IConcept<>), typeof (SpecificConcept),
                                                                           typeof (string)));
            Assert.IsFalse(GenericsPluginGraph.CanBePluggedIntoGenericType(typeof (IConcept<>), typeof (SpecificConcept),
                                                                           typeof (int)));
        }

        [Test]
        public void SimpleInstanceManagerTestWithGenerics()
        {
            var intService = (Service<int>) manager.GetInstance(typeof (IService<int>), "Default");
            Assert.AreEqual(typeof (int), intService.GetT());

            var stringService =
                (Service<string>) manager.GetInstance(typeof (IService<string>), "Default");
            Assert.AreEqual(typeof (string), stringService.GetT());
        }

        [Test]
        public void SingletonInterceptors()
        {
            var object1 = (AbstractClass<int>) manager.GetInstance(typeof (AbstractClass<int>));
            var object2 = (AbstractClass<int>) manager.GetInstance(typeof (AbstractClass<int>));
            var object3 = (AbstractClass<int>) manager.GetInstance(typeof (AbstractClass<int>));

            var object4 =
                (AbstractClass<string>) manager.GetInstance(typeof (AbstractClass<string>));
            var object5 =
                (AbstractClass<string>) manager.GetInstance(typeof (AbstractClass<string>));
            var object6 =
                (AbstractClass<string>) manager.GetInstance(typeof (AbstractClass<string>));

            Assert.AreSame(object1, object2);
            Assert.AreSame(object1, object3);
            Assert.AreSame(object4, object5);
            Assert.AreSame(object4, object6);

            Assert.AreNotSame(object1, object4);
        }

        [Test, Ignore("not sure we want this behavior anyway")]
        public void SpecificImplementation()
        {
            var concept = (IConcept<object>) manager.GetInstance(typeof (IConcept<object>), "Specific");

            Assert.IsNotNull(concept);
            Assert.IsInstanceOfType(typeof (SpecificConcept), concept);
        }

        interface IGenericType<T>{}
        class GenericType<T> : IGenericType<T> {}
        interface INonGenereic{}
        class NonGeneric : INonGenereic{}

        [Test]
        public void Can_use_factory_method_with_open_generics()
        {
            var container = new Container();
            container.Configure(x => x.For(typeof (IGenericType<>)).Use(f =>
            {
                var generic = f.BuildStack.Current.RequestedType.GetGenericArguments()[0];
                var type = typeof (GenericType<>).MakeGenericType(generic);
                return Activator.CreateInstance(type);
            }));

            var instance = container.GetInstance<IGenericType<string>>();
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.InstanceOfType(typeof(GenericType<string>)));
        }
    }
}