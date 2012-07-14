using System;
using System.Collections.Generic;
using System.Linq;
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

        [Test]
        public void Can_resolve_a_closed_generic_return_type()
        {
            container.Configure(cfg =>
            {
                cfg.For<IHandler<Message>>().Use<MessageHandler>();
                cfg.For<IDummyFactory>().CreateFactory();
            });

            var factory = container.GetInstance<IDummyFactory>();

            var component = factory.CreateHandler<Message>();

            component.ShouldNotBeNull();
            component.ShouldBeOfType<MessageHandler>();
        }

        [Test]
        public void It_can_name_instances_deep_in_the_object_graph()
        {
            container.Configure(cfg => cfg.For<IDummyFactory>().CreateFactory());

            var factory = container.GetInstance<IDummyFactory>();

            var message = new Message();
            var zombie = factory.CreateZombieService(message);

            zombie.MessageHandler.Message.ShouldBeTheSameAs(message);
        }

        [Test]
        public void It_can_name_instances()
        {
            container.Configure(cfg => cfg.For<IDummyFactory>().CreateFactory());

            var factory = container.GetInstance<IDummyFactory>();

            var message = new Message();
            var zombie = factory.CreateZombieMessageHandler(message);

            zombie.Message.ShouldBeTheSameAs(message);
        }

        [Test]
        public void It_can_name_instances_when_we_get_an_IEnumerable()
        {
            container.Configure(cfg =>
                {
                    cfg.For<IDummyFactory>().CreateFactory();
                    cfg.For<ZombieMessageHandler>().Use<ZombieMessageHandler>();
                });

            var factory = container.GetInstance<IDummyFactory>();

            var message = new Message();
            var zombies = factory.CreateZombieMessageHandlers(message);

            zombies.Count().ShouldEqual(1);
            foreach (var zomby in zombies)
            {
                zomby.Message.ShouldBeTheSameAs(message);
            }
        }
    }

    public interface IHandler<T>
    {
        void Handle(T thing);
    }

    public class Message { }

    public class MessageHandler : IHandler<Message>
    {
        public void Handle(Message thing)
        {
            
        }
    }

    public class ZombieMessageHandler
    {
        public readonly Message Message;

        public ZombieMessageHandler(Message thing)
        {
            Message = thing;
        }
    }

    public class ZombieService
    {
        public readonly ZombieMessageHandler MessageHandler;

        public ZombieService(ZombieMessageHandler messageHandler)
        {
            MessageHandler = messageHandler;
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
        ZombieMessageHandler CreateZombieMessageHandler(Message thing);
        IEnumerable<ZombieMessageHandler> CreateZombieMessageHandlers(Message thing);
        ZombieService CreateZombieService(Message thing);
    }
}