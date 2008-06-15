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
                registry.ScanAssemblies()
                    .IncludeAssembly("StructureMap.Testing.Widget")
                    .IncludeAssembly("StructureMap.Testing.Widget2");
            });
        }

        #endregion

        private Container _manager;


        [Test]
        public void CanMakeAClassWithNoConstructorParametersWithoutADefinedMemento()
        {
            Registry registry = new Registry();
            registry.ScanAssemblies().IncludeAssembly("StructureMap.Testing.Widget3");
            registry.BuildInstancesOf<IGateway>();

            PluginGraph graph = registry.Build();
            PipelineGraph pipelineGraph = new PipelineGraph(graph);

            BuildSession session = new BuildSession(graph);


            DefaultGateway gateway =
                (DefaultGateway) session.CreateInstance(typeof (IGateway), "Default");

            Assert.IsNotNull(gateway);
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void GetInstanceWithInvalidInstanceKey()
        {
            _manager.GetInstance<Rule>("NonExistentRule");
        }

        [Test]
        public void Import_from_family_picks_up_new_InstanceBuilders()
        {
            InstanceFactory factory = new InstanceFactory(typeof (IWidget));

            PluginFamily family = new PluginFamily(typeof (IWidget));
            family.AddPlugin(typeof (AWidget));
            family.AddPlugin(typeof (ColorWidget));
            factory.ImportFrom(family);

            InstanceBuilder builder = factory.FindBuilderByType(typeof (AWidget));
            Assert.IsNotNull(builder);

            builder = factory.FindBuilderByType(typeof (ColorWidget));
            Assert.IsNotNull(builder);
        }

        [Test]
        public void Import_from_family_picks_up_new_instances()
        {
            InstanceFactory factory = new InstanceFactory(typeof (IWidget));

            PluginFamily family = new PluginFamily(typeof (IWidget));
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
            InstanceFactory factory = new InstanceFactory(typeof (IWidget));
            LiteralInstance instance1 = new LiteralInstance(new AWidget()).WithName("New");
            factory.AddInstance(instance1);

            PluginFamily family = new PluginFamily(typeof (IWidget));
            family.AddInstance(new LiteralInstance(new AWidget()).WithName("New"));

            factory.ImportFrom(family);

            Assert.AreSame(instance1, factory.FindInstance("New"));
        }
    }
}