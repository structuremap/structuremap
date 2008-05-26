using System;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Exceptions;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using StructureMap.Source;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Container
{
    [TestFixture]
    public class InstanceManagerTester : Registry
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Registry registry = new Registry();
            registry.ScanAssemblies().IncludeAssembly("StructureMap.Testing.Widget");
            registry.BuildInstancesOf<Rule>();
            registry.BuildInstancesOf<IWidget>();
            registry.BuildInstancesOf<WidgetMaker>();

            _manager = registry.BuildInstanceManager();
        }

        #endregion

        private IInstanceManager _manager;

        private void addColorMemento(string Color)
        {
            ConfiguredInstance instance = new ConfiguredInstance(Color).WithConcreteKey("Color").SetProperty("Color", Color);
            
            _manager.AddInstance<Rule>(instance);
            _manager.AddInstance<IWidget>(instance);
            _manager.AddInstance<WidgetMaker>(instance);
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

        [Test]
        public void CanBuildConcreteTypesThatAreNotPreviouslyRegistered()
        {
            // Create a new InstanceManager that has a default instance configured for only the
            // IProvider interface.  InstanceManager is the real "container" behind ObjectFactory
            Registry registry = new Registry();
            registry.ForRequestedType<IProvider>().TheDefaultIsConcreteType<Provider>();
            InstanceManager manager = (InstanceManager) registry.BuildInstanceManager();

            // Now, have that same InstanceManager create a ClassThatUsesProvider.  StructureMap will
            // see that ClassThatUsesProvider is concrete, determine its constructor args, and build one 
            // for you with the default IProvider.  No other configuration necessary.
            ClassThatUsesProvider classThatUsesProvider = manager.CreateInstance<ClassThatUsesProvider>();
            Assert.IsInstanceOfType(typeof (Provider), classThatUsesProvider.Provider);
        }

        [Test]
        public void CanBuildConcreteTypesThatAreNotPreviouslyRegisteredWithArgumentsProvided()
        {
            Registry registry = new Registry();
            registry.ForRequestedType<IProvider>().TheDefaultIsConcreteType<Provider>();
            InstanceManager manager = (InstanceManager) registry.BuildInstanceManager();

            DifferentProvider differentProvider = new DifferentProvider();
            ExplicitArguments args = new ExplicitArguments();
            args.Set<IProvider>(differentProvider);

            ClassThatUsesProvider classThatUsesProvider = manager.CreateInstance<ClassThatUsesProvider>(args);
            Assert.AreSame(differentProvider, classThatUsesProvider.Provider);
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
            ColorRule rule = _manager.CreateInstance(typeof (Rule)) as ColorRule;

            Assert.IsNotNull(rule);
            Assert.AreEqual("Blue", rule.Color);
        }

        [Test]
        public void GetInstanceOf3Types()
        {
            addColorMemento("Red");
            addColorMemento("Orange");
            addColorMemento("Blue");

            ColorRule rule = _manager.CreateInstance(typeof (Rule), "Blue") as ColorRule;
            Assert.IsNotNull(rule);
            Assert.AreEqual("Blue", rule.Color);

            ColorWidget widget = _manager.CreateInstance(typeof (IWidget), "Red") as ColorWidget;
            Assert.IsNotNull(widget);
            Assert.AreEqual("Red", widget.Color);

            ColorWidgetMaker maker = _manager.CreateInstance(typeof (WidgetMaker), "Orange") as ColorWidgetMaker;
            Assert.IsNotNull(maker);
            Assert.AreEqual("Orange", maker.Color);
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void GetMissingType()
        {
            object o = _manager.CreateInstance(typeof (string));
        }


        private void assertColorIs(IInstanceManager manager, string color)
        {
            ColorService rule = (ColorService) manager.CreateInstance<IService>();
            Assert.AreEqual(color, rule.Color);
        }

        [Test]
        public void SetDefaultInstanceByString()
        {
            Registry registry = new Registry();
            registry.ForRequestedType<IService>()
                .AddInstance(Instance<ColorService>().WithName("Red").WithProperty("color").EqualTo("Red"))
                .AddInstance(Instance<ColorService>().WithName("Blue").WithProperty("color").EqualTo("Blue"))
                .AddInstance(Instance<ColorService>().WithName("Green").WithProperty("color").EqualTo("Green"));

            IInstanceManager manager = registry.BuildInstanceManager();
            manager.SetDefault(typeof(IService), "Red");
            assertColorIs(manager, "Red");

            manager.SetDefault(typeof(IService), "Green");
            assertColorIs(manager, "Green");

            manager.SetDefault(typeof(IService), "Blue");
            assertColorIs(manager, "Blue");
        }

        [Test]
        public void Can_set_profile_name_and_reset_defaults()
        {
            Registry registry = new Registry();
            registry.ForRequestedType<IService>()
                .TheDefaultIs(Instance<ColorService>().WithName("Orange").WithProperty("color").EqualTo("Orange"))
                .AddInstance(Instance<ColorService>().WithName("Red").WithProperty("color").EqualTo("Red"))
                .AddInstance(Instance<ColorService>().WithName("Blue").WithProperty("color").EqualTo("Blue"))
                .AddInstance(Instance<ColorService>().WithName("Green").WithProperty("color").EqualTo("Green"));

            registry.CreateProfile("Red").For<IService>().UseNamedInstance("Red");
            registry.CreateProfile("Blue").For<IService>().UseNamedInstance("Blue");

            IInstanceManager manager = registry.BuildInstanceManager();

            assertColorIs(manager, "Orange");

            manager.SetDefaultsToProfile("Red");
            assertColorIs(manager, "Red");

            manager.SetDefaultsToProfile("Blue");
            assertColorIs(manager, "Blue");
            
            manager.SetDefaultsToProfile(string.Empty);
            assertColorIs(manager, "Orange");
        }

        [Test, ExpectedException(typeof(StructureMapException))]
        public void TryToGetDefaultInstanceWithNoInstance()
        {
            InstanceManager manager = new InstanceManager(new PluginGraph());
            manager.CreateInstance<IService>();
        }

        [Test, ExpectedException(typeof(StructureMapConfigurationException))]
        public void CTOR_throws_StructureMapConfigurationException_if_there_is_an_error()
        {
            PluginGraph graph = new PluginGraph();
            graph.Log.RegisterError(400, new ApplicationException("Bad!"));

            new InstanceManager(graph);
        }
    }
}