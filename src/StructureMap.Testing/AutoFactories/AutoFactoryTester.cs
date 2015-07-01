using System;
using NUnit.Framework;
using Shouldly;
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
            container.Configure(cfg => {
                cfg.For<IDummyService>().Use<Dummy1>();
                cfg.For<IDummyFactory>().CreateFactory();
            });

            var factory = container.GetInstance<IDummyFactory>();

            factory.ShouldNotBeNull();
        }

        [Test]
        public void Can_resolve_component()
        {
            container.Configure(cfg => {
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
            container.Configure(cfg => {
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
            container.Configure(cfg => {
                cfg.For<IDummyService>().Use<Dummy1>();
                cfg.For<IDummyFactory>().CreateFactory();
            });

            var factory = container.GetInstance<IDummyFactory>();

            var component = factory.CreateService(typeof (IDummyService));

            component.ShouldNotBeNull();
            component.ShouldBeOfType<Dummy1>();
        }

        [Test]
        public void Can_resolve_a_closed_generic_return_type()
        {
            container.Configure(cfg => {
                cfg.For<IHandler<Message>>().Use<MessageHandler>();
                cfg.For<IDummyFactory>().CreateFactory();
            });

            var factory = container.GetInstance<IDummyFactory>();

            var component = factory.CreateHandler<Message>();

            component.ShouldNotBeNull();
            component.ShouldBeOfType<MessageHandler>();
        }
    }

    public interface IHandler<T>
    {
        void Handle(T thing);
    }

    public class Message
    {
    }

    public class MessageHandler : IHandler<Message>
    {
        public void Handle(Message thing)
        {
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
        IHandler<TMessage> CreateHandler<TMessage>();
        object CreateService(Type pluginType);
    }
}