using System;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.Mementos;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using StructureMap.Source;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Container
{
    [TestFixture]
    public class InstanceManagerTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            string[] assemblyNames = new string[] {"StructureMap.Testing.Widget"};

            _ruleFactory = ObjectMother.CreateInstanceFactory(typeof (Rule), assemblyNames);
            _widgetFactory = ObjectMother.CreateInstanceFactory(typeof (IWidget), assemblyNames);
            _widgetMakerFactory =
                ObjectMother.CreateInstanceFactory(typeof (WidgetMaker), assemblyNames);

            _manager = new InstanceManager();
            _manager.RegisterType(_ruleFactory);
            _manager.RegisterType(_widgetFactory);
            _manager.RegisterType(_widgetMakerFactory);
        }

        #endregion

        private InstanceManager _manager;
        private InstanceFactory _ruleFactory;
        private InstanceFactory _widgetFactory;
        private InstanceFactory _widgetMakerFactory;

        public InstanceManagerTester()
        {
        }


        private void addColorMemento(string Color)
        {
            ConfiguredInstance instance = new ConfiguredInstance(Color).WithConcreteKey("Color").SetProperty("Color", Color);
            
            _ruleFactory.AddInstance(instance);
            _widgetFactory.AddInstance(instance);
            _widgetMakerFactory.AddInstance(instance);
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
        public void CreatesInterceptionChainOnInstanceFactory()
        {
            // Create a PluginFamily with one Interceptor in the InterceptionChain of the Family
            PluginFamily family = new PluginFamily(typeof (Rule));
            SingletonInterceptor interceptor = new SingletonInterceptor();
            family.InterceptionChain.AddInterceptor(interceptor);

            // Create a PluginGraph with the PluginFamily
            PluginGraph pluginGraph = new PluginGraph();
            pluginGraph.Assemblies.Add("StructureMap.Testing.Widget");
            pluginGraph.PluginFamilies.Add(family);
            pluginGraph.Seal();

            // Create an InstanceManager and examine the InstanceFactory for the PluginFamily
            InstanceManager instanceManager = new InstanceManager(pluginGraph);

            IInstanceFactory wrappedFactory = instanceManager[typeof (Rule)];
            Assert.AreSame(interceptor, wrappedFactory);

            InstanceFactory factory = (InstanceFactory) interceptor.InnerInstanceFactory;
            Assert.AreEqual(typeof (Rule), factory.PluginType);
        }


        [Test]
        public void FindAPluginFamilyForAGenericTypeFromPluginTypeName()
        {
            Type serviceType = typeof (IService<string>);
            PluginGraph pluginGraph = PluginGraph.BuildGraphFromAssembly(serviceType.Assembly);

            InstanceManager manager = new InstanceManager(pluginGraph);

            Type stringService = typeof (IService<string>);

            IInstanceFactory factory = manager[stringService];
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

        [Test]
        public void PullsTheInstanceDefaultManagerFromPluginGraph()
        {
            Type serviceType = typeof (IService<>);
            PluginGraph pluginGraph = PluginGraph.BuildGraphFromAssembly(serviceType.Assembly);
            Assert.IsNotNull(pluginGraph.DefaultManager);

            InstanceManager manager = new InstanceManager(pluginGraph);
            Assert.AreSame(pluginGraph.DefaultManager, manager.DefaultManager);
        }
    }
}