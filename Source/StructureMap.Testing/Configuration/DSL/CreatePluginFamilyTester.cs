using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
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
                registry.BuildInstancesOfType<IGateway>().WithDefaultConcreteType<StubbedGateway>();
            }

            Assert.IsTrue(pluginGraph.PluginFamilies.Contains<IGateway>());

            InstanceManager manager = new InstanceManager(pluginGraph);
            IGateway gateway = (IGateway)manager.CreateInstance(typeof(IGateway));

            Assert.IsInstanceOfType(typeof(StubbedGateway), gateway);
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
                .PlugConcreteType<FakeGateway>().WithAlias("Fake")
                .PlugConcreteType<Fake2Gateway>()
                .PlugConcreteType<StubbedGateway>()
                .PlugConcreteType<Fake3Gateway>().WithAlias("Fake3");

            InstanceManager manager = registry.BuildInstanceManager();
            IGateway gateway = (IGateway)manager.CreateInstance(typeof(IGateway), "Fake");

            Assert.IsInstanceOfType(typeof(FakeGateway), gateway);
            Assert.IsInstanceOfType(typeof(Fake3Gateway), manager.CreateInstance(typeof(IGateway), "Fake3"));
            Assert.IsInstanceOfType(typeof(StubbedGateway), manager.CreateInstance(typeof(IGateway), "Stubbed"));
            Assert.IsInstanceOfType(typeof(Fake2Gateway), manager.CreateInstance(typeof(IGateway), TypePath.GetAssemblyQualifiedName(typeof(Fake2Gateway))));
        }
    }
}
