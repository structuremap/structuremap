using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
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
            _container = new Container(registry =>
            {
                registry.For<Rule>();
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

            registry.For<IGateway>();

            PluginGraph graph = registry.Build();
            var pipelineGraph = new PipelineGraph(graph);

            var session = new BuildSession(graph);

            var gateway =
                (DefaultGateway) session.CreateInstance(typeof (IGateway), "Default");

            Assert.IsNotNull(gateway);
        }

        [Test]
        public void do_not_replace_the_build_Lifecycle_if_it_is_the_same_type_as_the_imported_family()
        {
            var originalFamily = new PluginFamily(typeof (IWidget));
            originalFamily.SetScopeTo(InstanceScope.Singleton);
            var factory = new InstanceFactory(originalFamily);

            ILifecycle originalLifecycle = factory.Lifecycle;


            var family = new PluginFamily(typeof (IWidget));
            family.SetScopeTo(InstanceScope.Singleton);

            factory.ImportFrom(family);

            factory.Lifecycle.ShouldBeTheSameAs(originalLifecycle);
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

            factory.AllInstances.Count().ShouldEqual(0);

            lifecycle.AssertWasCalled(x => x.EjectAll());
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void GetInstanceWithInvalidInstanceKey()
        {
            _container.GetInstance<Rule>("NonExistentRule");
        }

        [Test]
        public void import_from_another_family_will_override_the_build_policy_if_the_initial_policy_is_the_default()
        {
            var factory = new InstanceFactory(typeof (IWidget));

            var family = new PluginFamily(typeof (IWidget));
            family.SetScopeTo(InstanceScope.Singleton);

            factory.ImportFrom(family);

            factory.Lifecycle.ShouldBeOfType<SingletonLifecycle>();
        }

        [Test]
        public void Import_from_family_picks_up_new_instances()
        {
            var factory = new InstanceFactory(typeof (IWidget));

            var family = new PluginFamily(typeof (IWidget));
            family.AddInstance(new ObjectInstance(new AWidget()).WithName("New"));
            family.AddInstance(new ObjectInstance(new AWidget()).WithName("New2"));
            family.AddInstance(new ObjectInstance(new AWidget()).WithName("New3"));

            factory.ImportFrom(family);

            Assert.IsNotNull(factory.FindInstance("New"));
            Assert.IsNotNull(factory.FindInstance("New2"));
            Assert.IsNotNull(factory.FindInstance("New3"));
        }


        [Test]
        public void Merge_from_PluginFamily_will_not_replace_an_existing_instance()
        {
            var factory = new InstanceFactory(typeof (IWidget));
            ObjectInstance instance1 = new ObjectInstance(new AWidget()).WithName("New");
            factory.AddInstance(instance1);

            var family = new PluginFamily(typeof (IWidget));
            family.AddInstance(new ObjectInstance(new AWidget()).WithName("New"));

            factory.ImportFrom(family);

            Assert.AreSame(instance1, factory.FindInstance("New"));
        }
    }

    [TestFixture]
    public class when_cloning_an_InstanceFactory
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            factory = new InstanceFactory(typeof (IGateway));
            factory.AddInstance(new SmartInstance<DefaultGateway>());
            factory.AddInstance(new SmartInstance<DefaultGateway>());

            var lifecycle = MockRepository.GenerateMock<ILifecycle>();
            factory.Lifecycle = lifecycle;
            factory.MissingInstance = new SmartInstance<DefaultGateway>();

            clone = factory.Clone();
        }

        #endregion

        private InstanceFactory factory;
        private IInstanceFactory clone;

        [Test]
        public void lifecycle_is_copied()
        {
            clone.Lifecycle.ShouldBeTheSameAs(factory.Lifecycle);
        }

        [Test]
        public void missing_instance_is_copied()
        {
            clone.MissingInstance.ShouldBeTheSameAs(factory.MissingInstance);
        }

        [Test]
        public void plugin_type_is_set_on_the_clone()
        {
            clone.PluginType.ShouldEqual(typeof (IGateway));
        }

        [Test]
        public void the_instances_are_cloned_so_that_new_instances_are_NOT_injected_into_()
        {
            clone.AddInstance(new ObjectInstance(new DefaultGateway()));

            factory.AllInstances.Count().ShouldEqual(2);
            clone.AllInstances.Count().ShouldEqual(3);
        }

        [Test]
        public void the_instances_are_copied()
        {
            clone.AllInstances.Count().ShouldEqual(2);
        }
    }
}