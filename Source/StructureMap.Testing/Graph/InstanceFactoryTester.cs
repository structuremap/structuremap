using NUnit.Framework;
using StructureMap.Attributes;
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
        public void import_from_another_family_will_override_the_build_policy_if_the_initial_policy_is_the_default()
        {
            var factory = new InstanceFactory(typeof(IWidget));

            var family = new PluginFamily(typeof(IWidget));
            family.SetScopeTo(InstanceScope.Singleton);

            factory.ImportFrom(family);

            factory.Policy.ShouldBeOfType<SingletonPolicy>();
        }

        [Test]
        public void do_not_replace_the_build_policy_if_it_is_the_same_type_as_the_imported_family()
        {
            PluginFamily originalFamily = new PluginFamily(typeof(IWidget));
            originalFamily.SetScopeTo(InstanceScope.Singleton);
            var factory = new InstanceFactory(originalFamily);

            var originalPolicy = factory.Policy;


            var family = new PluginFamily(typeof(IWidget));
            family.SetScopeTo(InstanceScope.Singleton);

            factory.ImportFrom(family);

            factory.Policy.ShouldBeTheSameAs(originalPolicy);
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