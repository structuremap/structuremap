using System;
using NUnit.Framework;
using StructureMap.AutoFactory;

namespace StructureMap.Testing.AutoFactories
{
    [TestFixture]
    public class AutoFactoryTester
    {

        private Container container;

        [SetUp]
        public void SetUp()
        {
            container = new Container();
        }

        [Test]
        public void Can_build_the_factory()
        {
            container.Configure(cfg =>
            {
                cfg.For<IDummyService>().Use<Dummy1>();
                cfg.For<IDummyFactory>().CreateFactory();
            });

            var factory = container.GetInstance<IDummyFactory>();

            factory.ShouldNotBeNull();
        }
        
        [Test]
        public void Can_resolve_component()
        {
            container.Configure(cfg =>
            {
                cfg.For<IDummyService>().Use<Dummy1>();
                cfg.For<IDummyFactory>().CreateFactory();
            });

            var factory = container.GetInstance<IDummyFactory>();

            var component = factory.CreateDummyService();

            component.ShouldNotBeNull();
            component.ShouldBeOfType<Dummy1>();
        }

        [Test]
        public void Can_resolve_generic_components_via_a_generic_method()
        {
            container.Configure(cfg =>
            {
                cfg.For<IDummyService>().Use<Dummy1>();
                cfg.For<IDummyFactory>().CreateFactory();
            });

            var factory = container.GetInstance<IDummyFactory>();

            var component = factory.CreateService<IDummyService>();

            component.ShouldNotBeNull();
            component.ShouldBeOfType<Dummy1>();
        }

        [Test]
        public void Can_resolve_components_via_a_non_generic_type_based_factory_method()
        {
            container.Configure(cfg =>
            {
                cfg.For<IDummyService>().Use<Dummy1>();
                cfg.For<IDummyFactory>().CreateFactory();
            });

            var factory = container.GetInstance<IDummyFactory>();

            var component = factory.CreateService(typeof (IDummyService));

            component.ShouldNotBeNull();
            component.ShouldBeOfType<Dummy1>();
        }
    }

    public interface IDummyService
    {
        string Name { get; set; }
    }

    public class Dummy1 : IDummyService
    {
        public string Name { get; set; }
    }

    public interface IDummyFactory
    {
        IDummyService CreateDummyService();
        TService CreateService<TService>();
        object CreateService(Type pluginType);
    }
}