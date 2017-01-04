using Shouldly;
using StructureMap.AutoFactory;
using System.Collections.Generic;
using Xunit;

namespace StructureMap.AutoFactory.Testing
{
    public class AutoFactoryTester
    {
        private readonly Container container;

        public AutoFactoryTester()
        {
            container = new Container();
        }

        [Fact]
        public void Can_build_the_factory()
        {
            container.Configure(cfg =>
            {
                cfg.For<IDummyService>().Use<DummyService>();
                cfg.For<IDummyFactory>().CreateFactory();
            });

            var factory = container.GetInstance<IDummyFactory>();

            factory.ShouldNotBeNull();
        }

        [Fact]
        public void Can_resolve_component()
        {
            container.Configure(cfg =>
            {
                cfg.For<IDummyService>().Use<DummyService>();
                cfg.For<IDummyFactory>().CreateFactory();
            });

            var factory = container.GetInstance<IDummyFactory>();

            var component = factory.CreateDummyService();

            component.ShouldNotBeNull();
            component.ShouldBeOfType<DummyService>();
        }

        [Fact]
        public void Can_resolve_generic_components_via_a_generic_method()
        {
            container.Configure(cfg =>
            {
                cfg.For<IDummyService>().Use<DummyService>();
                cfg.For<IDummyFactory>().CreateFactory();
            });

            var factory = container.GetInstance<IDummyFactory>();

            var component = factory.CreateService<IDummyService>();

            component.ShouldNotBeNull();
            component.ShouldBeOfType<DummyService>();
        }

        [Fact]
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

        [Fact]
        public void ResolveServiceWithExplicitArguments()
        {
            container.Configure(cfg =>
            {
                cfg.For<IDummyService>().Use<DummyServiceWithName>();
                cfg.For<IDummyFactory>().CreateFactory();
            });

            var factory = container.GetInstance<IDummyFactory>();

            var component = factory.CreateDummyService("John", "Smith");

            component.ShouldSatisfyAllConditions(
                () => component.ShouldNotBeNull(),
                () => component.ShouldBeOfType<DummyServiceWithName>(),
                () => component.Name.ShouldBe("John Smith")
            );
        }

        [Fact]
        public void ResolveServiceWithRedundantExplicitArguments()
        {
            container.Configure(cfg =>
            {
                cfg.For<IDummyService>().Use<DummyService>();
                cfg.For<IDummyFactory>().CreateFactory();
            });

            var factory = container.GetInstance<IDummyFactory>();

            var component = factory.CreateDummyService("John", "Smith");

            component.ShouldSatisfyAllConditions(
                () => component.ShouldNotBeNull(),
                () => component.ShouldBeOfType<DummyService>(),
                () => component.Name.ShouldBeNull()
            );
        }

        [Fact]
        public void ResolveNamedServiceWithExplicitArguments()
        {
            container.Configure(cfg =>
            {
                cfg.For<IDummyService>().Use<DummyService>();
                cfg.For<IDummyService>().Add<DummyServiceWithName>().Named("direct");
                cfg.For<IDummyService>().Add<DummyServiceWithReversedName>().Named("reversed");
                cfg.For<IDummyFactory>().CreateFactory();
            });

            var factory = container.GetInstance<IDummyFactory>();

            var component = factory.GetNamedDummyService("reversed", "John", "Smith");

            component.ShouldSatisfyAllConditions(
                () => component.ShouldNotBeNull(),
                () => component.ShouldBeOfType<DummyServiceWithReversedName>(),
                () => component.Name.ShouldBe("Smith John")
            );
        }

        [Fact]
        public void TryToResolveNotRegisteredNamedService()
        {
            container.Configure(cfg =>
            {
                cfg.For<IDummyService>().Use<DummyService>();
                cfg.For<IDummyService>().Add<DummyServiceWithName>().Named("direct");
                cfg.For<IDummyService>().Add<DummyServiceWithReversedName>().Named("reversed");
                cfg.For<IDummyFactory>().CreateFactory();
            });

            var factory = container.GetInstance<IDummyFactory>();

            var component = factory.GetNamedDummyService("unknown", "John", "Smith");

            component.ShouldBeNull();
        }

        [Fact]
        public void ResolveServiceNames()
        {
            container.Configure(cfg =>
            {
                cfg.For<IDummyService>().Use<DummyService>();
                cfg.For<IDummyService>().Add<DummyServiceWithName>().Named("direct");
                cfg.For<IDummyService>().Add<DummyServiceWithReversedName>().Named("reversed");
                cfg.For<IDummyFactory>().CreateFactory();
            });

            var factory = container.GetInstance<IDummyFactory>();

            var names = factory.GetNames<IDummyService>();

            names.ShouldBe(new[] { string.Empty, "direct", "reversed" }, true);
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

    // SAMPLE: IDummyService
    public interface IDummyService
    {
        string Name { get; }
    }

    // ENDSAMPLE

    // SAMPLE: DummyService
    public class DummyService : IDummyService
    {
        public string Name { get; set; }
    }

    // ENDSAMPLE

    public class DummyServiceWithName : IDummyService
    {
        public DummyServiceWithName(string namePart1, string namePart2)
        {
            Name = string.Format("{0} {1}", namePart1, namePart2);
        }

        public string Name { get; set; }
    }

    public class DummyServiceWithReversedName : IDummyService
    {
        public DummyServiceWithReversedName(string namePart1, string namePart2)
        {
            Name = string.Format("{0} {1}", namePart2, namePart1);
        }

        public string Name { get; set; }
    }

    // SAMPLE: IDummyFactory
    public interface IDummyFactory
    {
        // This method will return the names of all registered implementations of TService.
        IList<string> GetNames<TService>();

        // This method will just create the default IDummyService implementation.
        IDummyService CreateDummyService();

        // This method will just create the default IDummyService implementation and pass namePart1 and namePart2 as
        // dependencies.
        IDummyService CreateDummyService(string namePart1, string namePart2);

        // This method will create IDummyService implementation with serviceName name.
        IDummyService GetNamedDummyService(string serviceName, string namePart1, string namePart2);

        // Generic methods are also allowed as factory methods.
        TService CreateService<TService>();

        // Something that is common for event-sourcing implementations.
        IHandler<TMessage> CreateHandler<TMessage>();
    }

    // ENDSAMPLE
}