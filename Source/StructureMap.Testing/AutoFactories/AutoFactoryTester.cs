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
        public void Can_resolve_component()
        {
            container.Configure(cfg =>
            {
                cfg.For<IDummyService>().Use<Dummy1>();
                cfg.For<IDummyFactory>().CreateFactory();
            });

            var factory = container.GetInstance<IDummyFactory>();

            var component = factory.CreateDummyService();
            Assert.IsNotNull(component);
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
        IDummyService GetSecondService();
    }
}