using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class CreatePluginFamilyTester
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void TheDefaultInstanceIsConcreteType()
        {
            Registry registry = new Registry();

            // Needs to blow up if the concrete type can't be used
            registry.BuildInstancesOf<Rule>().TheDefaultIsConcreteType<ARule>();

            InstanceManager manager = registry.BuildInstanceManager();

            Assert.IsInstanceOfType(typeof(ARule), manager.CreateInstance<Rule>());
        }

        [Test]
        public void BuildInstancesOfType()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                registry.BuildInstancesOf<IGateway>();
            }

            Assert.IsTrue(pluginGraph.PluginFamilies.Contains<IGateway>());
        }

        [Test]
        public void TheDefaultInstanceIsPickedUpFromTheAttribute()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                registry.BuildInstancesOf<IGateway>();
            }

            Assert.IsTrue(pluginGraph.PluginFamilies.Contains<IGateway>());

            InstanceManager manager = new InstanceManager(pluginGraph);
            IGateway gateway = (IGateway)manager.CreateInstance(typeof(IGateway));

            Assert.IsInstanceOfType(typeof(DefaultGateway), gateway);
        }

        [Test]
        public void CanOverrideTheDefaultInstance1()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                // Specify the default implementation for an interface
                registry.BuildInstancesOf<IGateway>().WithDefaultConcreteType<StubbedGateway>();
            }

            Assert.IsTrue(pluginGraph.PluginFamilies.Contains<IGateway>());

            InstanceManager manager = new InstanceManager(pluginGraph);
            IGateway gateway = (IGateway)manager.CreateInstance(typeof(IGateway));

            Assert.IsInstanceOfType(typeof(StubbedGateway), gateway);
        }

        [Test]
        public void CreatePluginFamilyWithADefault()
        {
            Registry registry = new Registry();
            registry.BuildInstancesOf<IWidget>().TheDefaultIs(
                Registry.Instance<IWidget>().UsingConcreteType<ColorWidget>().WithProperty("Color").EqualTo("Red")
                );

            InstanceManager manager = registry.BuildInstanceManager();

            ColorWidget widget = (ColorWidget) manager.CreateInstance<IWidget>();
            Assert.AreEqual("Red", widget.Color);
        }


        [Test]
        public void BuildPluginFamilyAsPerRequest()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                CreatePluginFamilyExpression expression =
                    registry.BuildInstancesOf<IGateway>();
                Assert.IsNotNull(expression);
            }

            PluginFamily family = pluginGraph.PluginFamilies[typeof(IGateway)];
            Assert.AreEqual(0, family.InterceptionChain.Count);
        }

        [Test]
        public void AsAnotherScope()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                CreatePluginFamilyExpression expression = registry.BuildInstancesOf<IGateway>().CacheBy(InstanceScope.ThreadLocal);
                Assert.IsNotNull(expression);
            }

            PluginFamily family = pluginGraph.PluginFamilies[typeof(IGateway)];
            Assert.IsTrue(family.InterceptionChain.Contains(typeof(ThreadLocalStorageInterceptor)));
        }

        [Test]
        public void BuildPluginFamilyAsSingleton()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                CreatePluginFamilyExpression expression = 
                    registry.BuildInstancesOf<IGateway>().AsSingletons();
                Assert.IsNotNull(expression);
            }

            PluginFamily family = pluginGraph.PluginFamilies[typeof (IGateway)];
            Assert.IsTrue(family.InterceptionChain.Contains(typeof(SingletonInterceptor)));
        }

        [Test]
        public void CanOverrideTheDefaultInstanceAndCreateAnAllNewPluginOnTheFly()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                registry.BuildInstancesOf<IGateway>().WithDefaultConcreteType<FakeGateway>();
            }

            Assert.IsTrue(pluginGraph.PluginFamilies.Contains<IGateway>());

            InstanceManager manager = new InstanceManager(pluginGraph);
            IGateway gateway = (IGateway)manager.CreateInstance(typeof(IGateway));

            Assert.IsInstanceOfType(typeof(FakeGateway), gateway);
        }

    }
}
