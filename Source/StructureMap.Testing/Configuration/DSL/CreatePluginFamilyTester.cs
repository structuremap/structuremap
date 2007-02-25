using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Attributes;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture, Explicit]
    public class CreatePluginFamilyTester
    {
        [SetUp]
        public void SetUp()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                // Define the default instance of IWidget
                registry.BuildInstancesOfType<IWidget>().AndTheDefaultIs(
                    Registry.Instance<IWidget>()
                        .UsingConcreteType<ColorWidget>()
                        .WithProperty("Color").EqualTo("Red")
                    );

                
            }
        }

        [Test]
        public void BuildInstancesOfType()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                registry.BuildInstancesOfType<IGateway>();
            }

            Assert.IsTrue(pluginGraph.PluginFamilies.Contains<IGateway>());
        }

        [Test]
        public void TheDefaultInstanceIsPickedUpFromTheAttribute()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                registry.BuildInstancesOfType<IGateway>();
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
                registry.BuildInstancesOfType<IGateway>().WithDefaultConcreteType<StubbedGateway>();
            }

            Assert.IsTrue(pluginGraph.PluginFamilies.Contains<IGateway>());

            InstanceManager manager = new InstanceManager(pluginGraph);
            IGateway gateway = (IGateway)manager.CreateInstance(typeof(IGateway));

            Assert.IsInstanceOfType(typeof(StubbedGateway), gateway);
        }

        [Test]
        public void BuildPluginFamilyAsPerRequest()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                CreatePluginFamilyExpression expression =
                    registry.BuildInstancesOfType<IGateway>();
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
                CreatePluginFamilyExpression expression =
                    registry.BuildInstancesOfType<IGateway>().CacheInstanceAtScope(InstanceScope.ThreadLocal);
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
                    registry.BuildInstancesOfType<IGateway>().AsASingleton();
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
                registry.BuildInstancesOfType<IGateway>().WithDefaultConcreteType<FakeGateway>();
            }

            Assert.IsTrue(pluginGraph.PluginFamilies.Contains<IGateway>());

            InstanceManager manager = new InstanceManager(pluginGraph);
            IGateway gateway = (IGateway)manager.CreateInstance(typeof(IGateway));

            Assert.IsInstanceOfType(typeof(FakeGateway), gateway);
        }

        [Test]
        public void CanAddAdditionalConcreteTypes()
        {
            Registry registry = new Registry();

            registry.BuildInstancesOfType<IGateway>()
                .PluginConcreteType<FakeGateway>().AliasedAs("Fake")
                .PluginConcreteType<Fake2Gateway>()
                .PluginConcreteType<StubbedGateway>()
                .PluginConcreteType<Fake3Gateway>().AliasedAs("Fake3");

            InstanceManager manager = registry.BuildInstanceManager();
            IGateway gateway = (IGateway)manager.CreateInstance(typeof(IGateway), "Fake");

            Assert.IsInstanceOfType(typeof(FakeGateway), gateway);
            Assert.IsInstanceOfType(typeof(Fake3Gateway), manager.CreateInstance(typeof(IGateway), "Fake3"));
            Assert.IsInstanceOfType(typeof(StubbedGateway), manager.CreateInstance(typeof(IGateway), "Stubbed"));
            Assert.IsInstanceOfType(typeof(Fake2Gateway), manager.CreateInstance(typeof(IGateway), TypePath.GetAssemblyQualifiedName(typeof(Fake2Gateway))));
        }
    }
}
