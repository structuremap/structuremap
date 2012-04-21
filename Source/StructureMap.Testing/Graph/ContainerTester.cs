using System;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Exceptions;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.GenericWidgets;
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
                registry.BuildInstancesOf<Rule>();
                registry.BuildInstancesOf<IWidget>();
                registry.BuildInstancesOf<WidgetMaker>();
            });
        }

        #endregion

        private IContainer _container;

        private void addColorInstance(string Color)
        {
            _container.Configure(r =>
            {
                r.InstanceOf<Rule>().Is.OfConcreteType<ColorRule>().WithCtorArg("color").EqualTo(Color).WithName(Color);
                r.InstanceOf<IWidget>().Is.OfConcreteType<ColorWidget>().WithCtorArg("color").EqualTo(Color).WithName(
                    Color);
                r.InstanceOf<WidgetMaker>().Is.OfConcreteType<ColorWidgetMaker>().WithCtorArg("color").EqualTo(Color).
                    WithName(Color);
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


            public IProvider Provider { get { return _provider; } }
        }

        public class DifferentProvider : IProvider
        {
        }

        private void assertColorIs(IContainer container, string color)
        {
            container.GetInstance<IService>().ShouldBeOfType<ColorService>().Color.ShouldEqual(color);
        }

        [Test]
        public void Can_set_profile_name_and_reset_defaults()
        {
            var container = new Container(r =>
            {
                r.ForRequestedType<IService>()
                    .Use<ColorService>().WithName("Orange").WithProperty("color").EqualTo(
                    "Orange");

                r.ForRequestedType<IService>().AddInstances(x =>
                {
                    x.OfConcreteType<ColorService>().WithName("Red").WithProperty("color").EqualTo("Red");
                    x.OfConcreteType<ColorService>().WithName("Blue").WithProperty("color").EqualTo("Blue");
                    x.OfConcreteType<ColorService>().WithName("Green").WithProperty("color").EqualTo("Green");
                });

                r.Profile("Red").For<IService>().UseNamedInstance("Red");
                r.Profile("Blue").For<IService>().UseNamedInstance("Blue");
            });

            assertColorIs(container, "Orange");

            container.SetDefaultsToProfile("Red");
            assertColorIs(container, "Red");

            container.SetDefaultsToProfile("Blue");
            assertColorIs(container, "Blue");

            container.SetDefaultsToProfile(string.Empty);
            assertColorIs(container, "Orange");
        }

        [Test]
        public void CanBuildConcreteTypesThatAreNotPreviouslyRegistered()
        {
            IContainer manager = new Container(
                registry => registry.ForRequestedType<IProvider>().TheDefaultIsConcreteType<Provider>());

            // Now, have that same Container create a ClassThatUsesProvider.  StructureMap will
            // see that ClassThatUsesProvider is concrete, determine its constructor args, and build one 
            // for you with the default IProvider.  No other configuration necessary.
            var classThatUsesProvider = manager.GetInstance<ClassThatUsesProvider>();
            Assert.IsInstanceOfType(typeof (Provider), classThatUsesProvider.Provider);
        }

        [Test]
        public void CanBuildConcreteTypesThatAreNotPreviouslyRegisteredWithArgumentsProvided()
        {
            IContainer manager =
                new Container(
                    registry => registry.ForRequestedType<IProvider>().TheDefaultIsConcreteType<Provider>());

            var differentProvider = new DifferentProvider();
            var args = new ExplicitArguments();
            args.Set<IProvider>(differentProvider);

            var classThatUsesProvider = manager.GetInstance<ClassThatUsesProvider>(args);
            Assert.AreSame(differentProvider, classThatUsesProvider.Provider);
        }

        [Test, ExpectedException(typeof (StructureMapConfigurationException))]
        public void CTOR_throws_StructureMapConfigurationException_if_there_is_an_error()
        {
            var graph = new PluginGraph();
            graph.Log.RegisterError(400, new ApplicationException("Bad!"));

            new Container(graph);
        }


        [Test]
        public void FindAPluginFamilyForAGenericTypeFromPluginTypeName()
        {
            Type serviceType = typeof (IService<string>);
            PluginGraph pluginGraph = PluginGraph.BuildGraphFromAssembly(serviceType.Assembly);
            var pipelineGraph = new PipelineGraph(pluginGraph);

            Type stringService = typeof (IService<string>);

            IInstanceFactory factory = pipelineGraph.ForType(stringService);
            Assert.AreEqual(stringService, factory.PluginType);
        }

        [Test]
        public void GetDefaultInstance()
        {
            addColorInstance("Red");
            addColorInstance("Orange");
            addColorInstance("Blue");

            _container.Configure(x => { x.ForRequestedType<Rule>().TheDefault.Is.TheInstanceNamed("Blue"); });

            _container.GetInstance<Rule>().ShouldBeOfType<ColorRule>().Color.ShouldEqual("Blue");
        }

        [Test]
        public void GetInstanceOf3Types()
        {
            addColorInstance("Red");
            addColorInstance("Orange");
            addColorInstance("Blue");

            var rule = _container.GetInstance(typeof (Rule), "Blue") as ColorRule;
            Assert.IsNotNull(rule);
            Assert.AreEqual("Blue", rule.Color);

            var widget = _container.GetInstance(typeof (IWidget), "Red") as ColorWidget;
            Assert.IsNotNull(widget);
            Assert.AreEqual("Red", widget.Color);

            var maker = _container.GetInstance(typeof (WidgetMaker), "Orange") as ColorWidgetMaker;
            Assert.IsNotNull(maker);
            Assert.AreEqual("Orange", maker.Color);
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void GetMissingType()
        {
            object o = _container.GetInstance(typeof (string));
        }

        [Test]
        public void InjectStub_by_name()
        {
            IContainer container = new Container();

            var red = new ColorRule("Red");
            var blue = new ColorRule("Blue");

            container.Configure(x =>
            {
                x.For<Rule>().Add(red).WithName("Red");
                x.For<Rule>().Add(blue).WithName("Blue");
            });

            Assert.AreSame(red, container.GetInstance<Rule>("Red"));
            Assert.AreSame(blue, container.GetInstance<Rule>("Blue"));
        }

        [Test]
        public void TryGetInstance_returns_instance_for_an_open_generic_that_it_can_close()
        {
            var container =
                new Container(
                    x =>
                    x.ForRequestedType(typeof (IOpenGeneric<>)).TheDefaultIsConcreteType(typeof (ConcreteOpenGeneric<>)));
            container.TryGetInstance<IOpenGeneric<object>>().ShouldNotBeNull();
        }

        [Test]
        public void TryGetInstance_returns_null_for_an_open_generic_that_it_cannot_close()
        {
            var container =
                new Container(
                    x =>
                    x.ForRequestedType(typeof (IOpenGeneric<>)).TheDefaultIsConcreteType(typeof (ConcreteOpenGeneric<>)));
            container.TryGetInstance<IAnotherOpenGeneric<object>>().ShouldBeNull();
        }

        [Test]
        public void TryGetInstance_ReturnsInstance_WhenTypeFound()
        {
            _container.Configure(c => c.ForRequestedType<IProvider>().TheDefaultIsConcreteType<Provider>());
            object instance = _container.TryGetInstance(typeof (IProvider));
            instance.ShouldBeOfType(typeof (Provider));
        }

        [Test]
        public void TryGetInstance_ReturnsNull_WhenTypeNotFound()
        {
            object instance = _container.TryGetInstance(typeof (IProvider));
            instance.ShouldBeNull();
        }

        [Test]
        public void TryGetInstanceViaGeneric_ReturnsInstance_WhenTypeFound()
        {
            _container.Configure(c => c.ForRequestedType<IProvider>().TheDefaultIsConcreteType<Provider>());
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

            object rule = _container.TryGetInstance(typeof (Rule), "Yellow");
            rule.ShouldBeNull();
        }

        [Test]
        public void TryGetInstanceViaName_ReturnsTheOutInstance_WhenFound()
        {
            addColorInstance("Red");
            addColorInstance("Orange");
            addColorInstance("Blue");

            object rule = _container.TryGetInstance(typeof (Rule), "Orange");
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


        [Test, ExpectedException(typeof (StructureMapException))]
        public void TryToGetDefaultInstanceWithNoInstance()
        {
            var manager = new Container(new PluginGraph());
            manager.GetInstance<IService>();
        }
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