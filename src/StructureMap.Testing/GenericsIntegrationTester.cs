using NUnit.Framework;
using Shouldly;
using StructureMap.Testing.GenericWidgets;

namespace StructureMap.Testing
{
    [TestFixture]
    public class GenericsIntegrationTester
    {
        [SetUp]
        public void SetUp()
        {
            container = new Container(x =>
            {
                x.For(typeof (IThing<,>)).Use(typeof (ColorThing<,>)).Ctor<string>("color").Is("Red").Named("Red");
                x.For(typeof (IThing<,>)).Add(typeof (ComplexThing<,>))
                    .Ctor<string>("name").Is("Jeremy")
                    .Ctor<int>("age").Is(32)
                    .Ctor<bool>("ready").Is(true)
                    .Named("Complicated");

                x.For(typeof (ISimpleThing<>)).Use(typeof (SimpleThing<>));

                x.For(typeof (IService<>)).Use(typeof (Service<>)).Named("Default");

                x.For(typeof (AbstractClass<>)).Use(typeof (ConcreteClass<>));
                x.For(typeof (AbstractClass<>)).Singleton();
            });
        }

        private Container container;

        private interface IGenericType<T>
        {
        }

        private class GenericType<T> : IGenericType<T>
        {
        }

        private interface INonGenereic
        {
        }

        private class NonGeneric : INonGenereic
        {
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
        public void PicksUpASimpleGenericPluginFamilyFromConfiguration()
        {
            var thing = (ISimpleThing<int>) container.GetInstance(typeof (ISimpleThing<int>));
            thing.ShouldNotBeNull();
        }

        [Test]
        public void PicksUpAnExplicitlyDefinedGenericPluginFamilyFromConfiguration()
        {
            var thing =
                (IThing<int, string>) container.GetInstance(typeof (IThing<int, string>));
            var redThing = (ColorThing<int, string>) thing;

            redThing.Color.ShouldBe("Red");
        }

        [Test]
        public void SimpleInstanceManagerTestWithGenerics()
        {
            var intService = (Service<int>) container.GetInstance(typeof (IService<int>), "Default");
            intService.GetT().ShouldBe(typeof (int));

            container.GetInstance(typeof (IService<string>), "Default")
                .As<Service<string>>().GetT().ShouldBe(typeof (string));
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

            object1.ShouldBeTheSameAs(object2);
            object1.ShouldBeTheSameAs(object3);
            object4.ShouldBeTheSameAs(object5);
            object4.ShouldBeTheSameAs(object6);

            object1.ShouldNotBeTheSameAs(object4);
        }
    }
}