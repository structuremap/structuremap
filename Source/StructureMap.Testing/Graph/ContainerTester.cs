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
            _manager = new Container(registry =>
            {
                registry.ScanAssemblies().IncludeAssembly("StructureMap.Testing.Widget");
                registry.BuildInstancesOf<Rule>();
                registry.BuildInstancesOf<IWidget>();
                registry.BuildInstancesOf<WidgetMaker>();
            });
        }

        #endregion

        private IContainer _manager;

        private void addColorMemento(string Color)
        {
            _manager.Configure(registry =>
            {
                registry.AddInstanceOf<Rule>().UsingConcreteType<ColorRule>().SetProperty("color", Color).WithName(Color);
                registry.AddInstanceOf<IWidget>().UsingConcreteType<ColorWidget>().SetProperty("color", Color).WithName(
                    Color);
                registry.AddInstanceOf<WidgetMaker>().UsingConcreteType<ColorWidgetMaker>().SetProperty("color", Color).
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


            public IProvider Provider
            {
                get { return _provider; }
            }
        }

        public class DifferentProvider : IProvider
        {
        }

        private void assertColorIs(IContainer manager, string color)
        {
            ColorService rule = (ColorService) manager.GetInstance<IService>();
            Assert.AreEqual(color, rule.Color);
        }

        [Test]
        public void Can_set_profile_name_and_reset_defaults()
        {
            IContainer manager = new Container(registry =>
            {
                registry.ForRequestedType<IService>()
                    .TheDefaultIs(Instance<ColorService>().WithName("Orange").WithProperty("color").EqualTo("Orange"))
                    .AddInstance(Instance<ColorService>().WithName("Red").WithProperty("color").EqualTo("Red"))
                    .AddInstance(Instance<ColorService>().WithName("Blue").WithProperty("color").EqualTo("Blue"))
                    .AddInstance(Instance<ColorService>().WithName("Green").WithProperty("color").EqualTo("Green"));

                registry.CreateProfile("Red").For<IService>().UseNamedInstance("Red");
                registry.CreateProfile("Blue").For<IService>().UseNamedInstance("Blue");
            });

            assertColorIs(manager, "Orange");

            manager.SetDefaultsToProfile("Red");
            assertColorIs(manager, "Red");

            manager.SetDefaultsToProfile("Blue");
            assertColorIs(manager, "Blue");

            manager.SetDefaultsToProfile(string.Empty);
            assertColorIs(manager, "Orange");
        }

        [Test]
        public void CanBuildConcreteTypesThatAreNotPreviouslyRegistered()
        {
            IContainer manager = new Container(
                registry => registry.ForRequestedType<IProvider>().TheDefaultIsConcreteType<Provider>());

            // Now, have that same Container create a ClassThatUsesProvider.  StructureMap will
            // see that ClassThatUsesProvider is concrete, determine its constructor args, and build one 
            // for you with the default IProvider.  No other configuration necessary.
            ClassThatUsesProvider classThatUsesProvider = manager.GetInstance<ClassThatUsesProvider>();
            Assert.IsInstanceOfType(typeof (Provider), classThatUsesProvider.Provider);
        }

        [Test]
        public void CanBuildConcreteTypesThatAreNotPreviouslyRegisteredWithArgumentsProvided()
        {
            IContainer manager =
                new Container(
                    registry => registry.ForRequestedType<IProvider>().TheDefaultIsConcreteType<Provider>());

            DifferentProvider differentProvider = new DifferentProvider();
            ExplicitArguments args = new ExplicitArguments();
            args.Set<IProvider>(differentProvider);

            ClassThatUsesProvider classThatUsesProvider = manager.GetInstance<ClassThatUsesProvider>(args);
            Assert.AreSame(differentProvider, classThatUsesProvider.Provider);
        }

        [Test, ExpectedException(typeof (StructureMapConfigurationException))]
        public void CTOR_throws_StructureMapConfigurationException_if_there_is_an_error()
        {
            PluginGraph graph = new PluginGraph();
            graph.Log.RegisterError(400, new ApplicationException("Bad!"));

            new Container(graph);
        }


        [Test]
        public void FindAPluginFamilyForAGenericTypeFromPluginTypeName()
        {
            Type serviceType = typeof (IService<string>);
            PluginGraph pluginGraph = PluginGraph.BuildGraphFromAssembly(serviceType.Assembly);
            PipelineGraph pipelineGraph = new PipelineGraph(pluginGraph);

            Type stringService = typeof (IService<string>);

            IInstanceFactory factory = pipelineGraph.ForType(stringService);
            Assert.AreEqual(stringService, factory.PluginType);
        }

        [Test]
        public void GetDefaultInstance()
        {
            addColorMemento("Red");
            addColorMemento("Orange");
            addColorMemento("Blue");

            _manager.SetDefault(typeof (Rule), "Blue");
            ColorRule rule = _manager.GetInstance(typeof (Rule)) as ColorRule;

            Assert.IsNotNull(rule);
            Assert.AreEqual("Blue", rule.Color);
        }

        [Test]
        public void GetInstanceOf3Types()
        {
            addColorMemento("Red");
            addColorMemento("Orange");
            addColorMemento("Blue");

            ColorRule rule = _manager.GetInstance(typeof (Rule), "Blue") as ColorRule;
            Assert.IsNotNull(rule);
            Assert.AreEqual("Blue", rule.Color);

            ColorWidget widget = _manager.GetInstance(typeof (IWidget), "Red") as ColorWidget;
            Assert.IsNotNull(widget);
            Assert.AreEqual("Red", widget.Color);

            ColorWidgetMaker maker = _manager.GetInstance(typeof (WidgetMaker), "Orange") as ColorWidgetMaker;
            Assert.IsNotNull(maker);
            Assert.AreEqual("Orange", maker.Color);
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void GetMissingType()
        {
            object o = _manager.GetInstance(typeof (string));
        }

        [Test]
        public void InjectStub_by_name()
        {
            IContainer container = new Container();

            ColorRule red = new ColorRule("Red");
            ColorRule blue = new ColorRule("Blue");

            container.Inject<Rule>("Red", red);
            container.Inject<Rule>("Blue", blue);

            Assert.AreSame(red, container.GetInstance<Rule>("Red"));
            Assert.AreSame(blue, container.GetInstance<Rule>("Blue"));
        }


        [Test]
        public void SetDefaultInstanceByString()
        {
            IContainer manager = new Container(registry => registry.ForRequestedType<IService>()
                                                               .AddInstance(
                                                               Instance<ColorService>().WithName("Red").WithProperty(
                                                                   "color").EqualTo("Red"))
                                                               .AddInstance(
                                                               Instance<ColorService>().WithName("Blue").WithProperty(
                                                                   "color").EqualTo("Blue"))
                                                               .AddInstance(
                                                               Instance<ColorService>().WithName("Green").WithProperty(
                                                                   "color").EqualTo("Green")));

            manager.SetDefault(typeof (IService), "Red");
            assertColorIs(manager, "Red");

            manager.SetDefault(typeof (IService), "Green");
            assertColorIs(manager, "Green");

            manager.SetDefault(typeof (IService), "Blue");
            assertColorIs(manager, "Blue");
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void TryToGetDefaultInstanceWithNoInstance()
        {
            Container manager = new Container(new PluginGraph());
            manager.GetInstance<IService>();
        }


    }


}