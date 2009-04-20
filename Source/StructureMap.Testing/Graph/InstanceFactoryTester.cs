using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Attributes;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;
using System.Linq;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class InstanceFactoryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _container = new Container(registry =>
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

        private Container _container;


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
            _container.GetInstance<Rule>("NonExistentRule");
        }

        [Test]
        public void eject_all_instances_removes_all_instances_and_ejects_from_the_build_policy()
        {
            var factory = new InstanceFactory(typeof (IGateway));
            factory.AddInstance(new SmartInstance<DefaultGateway>());
            factory.AddInstance(new SmartInstance<DefaultGateway>());

            var lifecycle = MockRepository.GenerateMock<ILifecycle>();
            factory.Lifecycle = lifecycle;

            factory.EjectAllInstances();

            factory.Instances.Count().ShouldEqual(0);

            lifecycle.AssertWasCalled(x => x.EjectAll());
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

            factory.Lifecycle.ShouldBeOfType<SingletonLifecycle>();
        }

        [Test]
        public void do_not_replace_the_build_Lifecycle_if_it_is_the_same_type_as_the_imported_family()
        {
            PluginFamily originalFamily = new PluginFamily(typeof(IWidget));
            originalFamily.SetScopeTo(InstanceScope.Singleton);
            var factory = new InstanceFactory(originalFamily);

            var originalLifecycle = factory.Lifecycle;


            var family = new PluginFamily(typeof(IWidget));
            family.SetScopeTo(InstanceScope.Singleton);

            factory.ImportFrom(family);

            factory.Lifecycle.ShouldBeTheSameAs(originalLifecycle);
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