using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class InstanceFactoryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _manager = new Container(registry =>
            {
                registry.BuildInstancesOf<Rule>();
                registry.Scan(x =>
                {
                    x.Assembly("StructureMap.Testing.Widget");
                    x.Assembly("StructureMap.Testing.Widget2");
                });
            });
        }

        #endregion

        private Container _manager;


        [Test]
        public void CanMakeAClassWithNoConstructorParametersWithoutADefinedMemento()
        {
            var registry = new Registry();
            registry.Scan(x => x.Assembly("StructureMap.Testing.Widget3"));

            registry.BuildInstancesOf<IGateway>();

            PluginGraph graph = registry.Build();
            var pipelineGraph = new PipelineGraph(graph);

            var session = new BuildSession(graph);

            var gateway =
                (DefaultGateway) session.CreateInstance(typeof (IGateway), "Default");

            Assert.IsNotNull(gateway);
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void GetInstanceWithInvalidInstanceKey()
        {
            _manager.GetInstance<Rule>("NonExistentRule");
        }

        [Test]
        public void Import_from_family_picks_up_new_instances()
        {
            var factory = new InstanceFactory(typeof (IWidget));

            var family = new PluginFamily(typeof (IWidget));
            family.AddInstance(new LiteralInstance(new AWidget()).WithName("New"));
            family.AddInstance(new LiteralInstance(new AWidget()).WithName("New2"));
            family.AddInstance(new LiteralInstance(new AWidget()).WithName("New3"));

            factory.ImportFrom(family);

            Assert.IsNotNull(factory.FindInstance("New"));
            Assert.IsNotNull(factory.FindInstance("New2"));
            Assert.IsNotNull(factory.FindInstance("New3"));
        }

        [Test]
        public void Merge_from_PluginFamily_will_not_replace_an_existing_instance()
        {
            var factory = new InstanceFactory(typeof (IWidget));
            LiteralInstance instance1 = new LiteralInstance(new AWidget()).WithName("New");
            factory.AddInstance(instance1);

            var family = new PluginFamily(typeof (IWidget));
            family.AddInstance(new LiteralInstance(new AWidget()).WithName("New"));

            factory.ImportFrom(family);

            Assert.AreSame(instance1, factory.FindInstance("New"));
        }
    }
}