using NUnit.Framework;
using Shouldly;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class ContainerTester : Registry
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _container = new Container(registry =>
            {
                registry.Scan(x => x.Assembly("StructureMap.Testing.Widget"));
                registry.For<Rule>();
                registry.For<IWidget>();
                registry.For<WidgetMaker>();
            });
        }

        #endregion

        private IContainer _container;

        private void addColorInstance(string Color)
        {
            _container.Configure(r =>
            {
                r.For<Rule>().Use<ColorRule>().Ctor<string>("color").Is(Color).Named(Color);
                r.For<IWidget>().Use<ColorWidget>().Ctor<string>("color").Is(Color).Named(
                    Color);
                r.For<WidgetMaker>().Use<ColorWidgetMaker>().Ctor<string>("color").Is(Color).
                    Named(Color);
            });
        }

        public interface IProvider
        {
        }

        public class Provider : IProvider
        {
        }

        public class ClassThatUsesProvider
        {
            private readonly IProvider _provider;

            public ClassThatUsesProvider(IProvider provider)
            {
                _provider = provider;
            }


            public IProvider Provider
            {
                get { return _provider; }
            }
        }

        public class DifferentProvider : IProvider
        {
        }

        private void assertColorIs(IContainer container, string color)
        {
            container.GetInstance<IService>().ShouldBeOfType<ColorService>().Color.ShouldBe(color);
        }

        [Test]
        public void can_inject_into_a_running_container()
        {
            var container = new Container();
            container.Inject(typeof (ISport), new ConstructorInstance(typeof (Football)));

            container.GetInstance<ISport>()
                .ShouldBeOfType<Football>();
        }

        [Test]
        public void Can_set_profile_name_and_reset_defaults()
        {
            var container = new Container(r =>
            {
                r.For<IService>()
                    .Use<ColorService>().Named("Orange").Ctor<string>("color").Is(
                        "Orange");

                r.For<IService>().AddInstances(x =>
                {
                    x.Type<ColorService>().Named("Red").Ctor<string>("color").Is("Red");
                    x.Type<ColorService>().Named("Blue").Ctor<string>("color").Is("Blue");
                    x.Type<ColorService>().Named("Green").Ctor<string>("color").Is("Green");
                });

                r.Profile("Red", x => { x.For<IService>().Use("Red"); });

                r.Profile("Blue", x => { x.For<IService>().Use("Blue"); });
            });

            assertColorIs(container, "Orange");

            assertColorIs(container.GetProfile("Red"), "Red");

            assertColorIs(container.GetProfile("Blue"), "Blue");

            assertColorIs(container, "Orange");
        }

        [Test]
        public void CanBuildConcreteTypesThatAreNotPreviouslyRegistered()
        {
            IContainer manager = new Container(
                registry => registry.For<IProvider>().Use<Provider>());

            // Now, have that same Container create a ClassThatUsesProvider.  StructureMap will
            // see that ClassThatUsesProvider is concrete, determine its constructor args, and build one 
            // for you with the default IProvider.  No other configuration necessary.
            var classThatUsesProvider = manager.GetInstance<ClassThatUsesProvider>();

            classThatUsesProvider.Provider.ShouldBeOfType<Provider>();
        }

        [Test]
        public void CanBuildConcreteTypesThatAreNotPreviouslyRegisteredWithArgumentsProvided()
        {
            IContainer manager =
                new Container(
                    registry => registry.For<IProvider>().Use<Provider>());

            var differentProvider = new DifferentProvider();
            var args = new ExplicitArguments();
            args.Set<IProvider>(differentProvider);

            var classThatUsesProvider = manager.GetInstance<ClassThatUsesProvider>(args);
            differentProvider.AreSame(classThatUsesProvider.Provider);
        }


        [Test]
        public void can_get_the_default_instance()
        {
            addColorInstance("Red");
            addColorInstance("Orange");
            addColorInstance("Blue");

            _container.Configure(x => x.For<Rule>().Use("Blue"));

            _container.GetInstance<Rule>().ShouldBeOfType<ColorRule>().Color.ShouldBe("Blue");
        }

        [Test]
        public void GetInstanceOf3Types()
        {
            addColorInstance("Red");
            addColorInstance("Orange");
            addColorInstance("Blue");

            var rule = _container.GetInstance(typeof (Rule), "Blue") as ColorRule;
            rule.IsNotNull();
            rule.Color.ShouldBe("Blue");

            var widget = _container.GetInstance(typeof (IWidget), "Red") as ColorWidget;
            widget.IsNotNull();
            widget.Color.ShouldBe("Red");

            var maker = _container.GetInstance(typeof (WidgetMaker), "Orange") as ColorWidgetMaker;
            maker.IsNotNull();
            maker.Color.ShouldBe("Orange");
        }

        [Test]
        public void GetMissingType()
        {
            var ex =
                Exception<StructureMapConfigurationException>.ShouldBeThrownBy(
                    () => { _container.GetInstance(typeof (string)); });

            ex.Title.ShouldContain("No default");
        }

        [Test]
        public void InjectStub_by_name()
        {
            IContainer container = new Container();

            var red = new ColorRule("Red");
            var blue = new ColorRule("Blue");

            container.Configure(x =>
            {
                x.For<Rule>().Add(red).Named("Red");
                x.For<Rule>().Add(blue).Named("Blue");
            });

            red.AreSame(container.GetInstance<Rule>("Red"));
            blue.AreSame(container.GetInstance<Rule>("Blue"));
        }

        [Test]
        public void TryGetInstance_returns_instance_for_an_open_generic_that_it_can_close()
        {
            var container =
                new Container(
                    x =>
                        x.For(typeof (IOpenGeneric<>)).Use(typeof (ConcreteOpenGeneric<>)));
            container.TryGetInstance<IOpenGeneric<object>>().ShouldNotBeNull();
        }

        [Test]
        public void TryGetInstance_returns_null_for_an_open_generic_that_it_cannot_close()
        {
            var container =
                new Container(
                    x =>
                        x.For(typeof (IOpenGeneric<>)).Use(typeof (ConcreteOpenGeneric<>)));
            container.TryGetInstance<IAnotherOpenGeneric<object>>().ShouldBeNull();
        }

        [Test]
        public void TryGetInstance_ReturnsInstance_WhenTypeFound()
        {
            _container.Configure(c => c.For<IProvider>().Use<Provider>());
            var instance = _container.TryGetInstance(typeof (IProvider));
            instance.ShouldBeOfType(typeof (Provider));
        }

        [Test]
        public void TryGetInstance_ReturnsNull_WhenTypeNotFound()
        {
            var instance = _container.TryGetInstance(typeof (IProvider));
            instance.ShouldBeNull();
        }

        [Test]
        public void TryGetInstanceViaGeneric_ReturnsInstance_WhenTypeFound()
        {
            _container.Configure(c => c.For<IProvider>().Use<Provider>());
            var instance = _container.TryGetInstance<IProvider>();
            instance.ShouldBeOfType(typeof (Provider));
        }

        [Test]
        public void TryGetInstanceViaGeneric_ReturnsNull_WhenTypeNotFound()
        {
            var instance = _container.TryGetInstance<IProvider>();
            instance.ShouldBeNull();
        }

        [Test]
        public void TryGetInstanceViaName_ReturnsNull_WhenNotFound()
        {
            addColorInstance("Red");
            addColorInstance("Orange");
            addColorInstance("Blue");

            var rule = _container.TryGetInstance(typeof (Rule), "Yellow");
            rule.ShouldBeNull();
        }

        [Test]
        public void TryGetInstanceViaName_ReturnsTheOutInstance_WhenFound()
        {
            addColorInstance("Red");
            addColorInstance("Orange");
            addColorInstance("Blue");

            var rule = _container.TryGetInstance(typeof (Rule), "Orange");
            rule.ShouldBeOfType(typeof (ColorRule));
        }

        [Test]
        public void TryGetInstanceViaNameAndGeneric_ReturnsInstance_WhenTypeFound()
        {
            addColorInstance("Red");
            addColorInstance("Orange");
            addColorInstance("Blue");

            // "Orange" exists, so an object should be returned
            var instance = _container.TryGetInstance<Rule>("Orange");
            instance.ShouldBeOfType(typeof (ColorRule));
        }

        [Test]
        public void TryGetInstanceViaNameAndGeneric_ReturnsNull_WhenTypeNotFound()
        {
            addColorInstance("Red");
            addColorInstance("Orange");
            addColorInstance("Blue");

            // "Yellow" does not exist, so return null
            var instance = _container.TryGetInstance<Rule>("Yellow");
            instance.ShouldBeNull();
        }
    }

    public interface ISport
    {
    }

    public class Football : ISport
    {
    }

    public interface IOpenGeneric<T>
    {
        void Nop();
    }

    public interface IAnotherOpenGeneric<T>
    {
    }

    public class ConcreteOpenGeneric<T> : IOpenGeneric<T>
    {
        public void Nop()
        {
        }
    }

    public class StringOpenGeneric : ConcreteOpenGeneric<string>
    {
    }
}