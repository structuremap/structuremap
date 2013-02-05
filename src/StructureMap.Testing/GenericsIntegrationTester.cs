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
            container = new Container(x => {
                x.For(typeof (IThing<,>)).Use(typeof (ColorThing<,>)).Ctor<string>("color").Is("Red").Named("Red");
                x.For(typeof (IThing<,>)).Add(typeof (ComplexThing<,>))
                    .Ctor<string>("name").Is("Jeremy")
                    .Ctor<int>("age").Is(32)
                    .Ctor<bool>("ready").Is(true)
                    .Named("Complicated");

                x.For(typeof (ISimpleThing<>)).Use(typeof (SimpleThing<>));

                x.For(typeof (IService<>)).Use(typeof (Service<>)).Named("Default");

                x.For(typeof (AbstractClass<>)).Use(typeof(ConcreteClass<>));
            });
        }

        #endregion

        private Container container;

        [Test, Ignore("not sure I want this behavior anyway")]
        public void AllTypesWithSpecificImplementation()
        {
            IList objectConcepts = container.GetAllInstances(typeof (IConcept<object>));

            Assert.IsNotNull(objectConcepts);
            Assert.AreEqual(2, objectConcepts.Count);

            IList stringConcepts = container.GetAllInstances(typeof (IConcept<string>));

            Assert.IsNotNull(stringConcepts);
            Assert.AreEqual(1, stringConcepts.Count);
        }


        [Test]
        public void MultipleGenericTypes()
        {
            var intService = (IService<int>) container.GetInstance(typeof (IService<int>), "Default");
            var stringService =
                (IService<string>) container.GetInstance(typeof (IService<string>), "Default");
            var doubleService =
                (IService<double>) container.GetInstance(typeof (IService<double>), "Default");
        }


        [Test]
        public void PicksUpAnExplicitlyDefinedGenericPluginFamilyFromConfiguration()
        {
            var thing =
                (IThing<int, string>) container.GetInstance(typeof (IThing<int, string>));
            var redThing = (ColorThing<int, string>) thing;

            Assert.AreEqual("Red", redThing.Color);
        }

        [Test]
        public void PicksUpASimpleGenericPluginFamilyFromConfiguration()
        {
            var thing = (ISimpleThing<int>) container.GetInstance(typeof (ISimpleThing<int>));
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
            var intService = (Service<int>) container.GetInstance(typeof (IService<int>), "Default");
            Assert.AreEqual(typeof (int), intService.GetT());

            var stringService =
                (Service<string>) container.GetInstance(typeof (IService<string>), "Default");
            Assert.AreEqual(typeof (string), stringService.GetT());
        }

        [Test]
        public void SingletonInterceptors()
        {
            var object1 = (AbstractClass<int>) container.GetInstance(typeof (AbstractClass<int>));
            var object2 = (AbstractClass<int>) container.GetInstance(typeof (AbstractClass<int>));
            var object3 = (AbstractClass<int>) container.GetInstance(typeof (AbstractClass<int>));

            var object4 =
                (AbstractClass<string>) container.GetInstance(typeof (AbstractClass<string>));
            var object5 =
                (AbstractClass<string>) container.GetInstance(typeof (AbstractClass<string>));
            var object6 =
                (AbstractClass<string>) container.GetInstance(typeof (AbstractClass<string>));

            Assert.AreSame(object1, object2);
            Assert.AreSame(object1, object3);
            Assert.AreSame(object4, object5);
            Assert.AreSame(object4, object6);

            Assert.AreNotSame(object1, object4);
        }

        [Test, Ignore("not sure we want this behavior anyway")]
        public void SpecificImplementation()
        {
            var concept = (IConcept<object>) container.GetInstance(typeof (IConcept<object>), "Specific");

            concept.ShouldNotBeNull();
            concept.ShouldBeOfType<SpecificConcept>();
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