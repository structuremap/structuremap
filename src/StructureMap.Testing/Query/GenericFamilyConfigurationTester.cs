using System.Linq;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Query;
using StructureMap.Testing.GenericWidgets;

namespace StructureMap.Testing.Query
{
    [TestFixture]
    public class GenericFamilyConfigurationTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            family = new PluginFamily(typeof (IService<>));
            new PluginGraph("something").AddFamily(family);

            configuration = new GenericFamilyConfiguration(family);
        }

        #endregion

        private PluginFamily family;
        private GenericFamilyConfiguration configuration;

        [Test]
        public void profile_name_is_taken_from_PluginGraph()
        {
            configuration.ProfileName.ShouldEqual("something");
        }

        [Test]
        public void build_should_return_null()
        {
            configuration.As<IFamily>().Build(null).ShouldBeNull();
        }

        [Test]
        public void eject_and_remove_an_instance()
        {
            var instance = new ConfiguredInstance(typeof (Service<>));
            family.AddInstance(instance);
            family.AddInstance(new ConfiguredInstance(typeof (Service2<>)));

            InstanceRef iRef = configuration.Instances.FirstOrDefault(x => x.Name == instance.Name);

            configuration.EjectAndRemove(iRef);

            family.Instances.Count().ShouldEqual(1);
            configuration.Instances.Count().ShouldEqual(1);

            configuration.Instances.Any(x => x.Name == instance.Name).ShouldBeFalse();
        }

        [Test]
        public void eject_and_remove_the_default_value()
        {
            var instance = new ConfiguredInstance(typeof (Service<>));
            family.SetDefault(instance);
            var secondInstance = new ConfiguredInstance(typeof (Service2<>));
            family.AddInstance(secondInstance);

            InstanceRef iRef = configuration.Instances.FirstOrDefault(x => x.Name == instance.Name);

            configuration.EjectAndRemove(iRef);

            family.GetDefaultInstance().ShouldBeTheSameAs(secondInstance);
        }

        [Test]
        public void eject_does_nothing_and_does_not_blow_up()
        {
            configuration.As<IFamily>().Eject(null);
        }

        [Test]
        public void get_instances()
        {
            family.AddInstance(new ConfiguredInstance(typeof (Service<>)));
            family.AddInstance(new ConfiguredInstance(typeof (Service2<>)));

            configuration.Instances.Select(x => x.ConcreteType)
                .ShouldHaveTheSameElementsAs(typeof (Service<>), typeof (Service2<>));
        }

        [Test]
        public void get_the_default_instance_when_it_does_not_exist()
        {
            configuration.Default.ShouldBeNull();
        }

        [Test]
        public void get_the_default_instance_when_it_exists()
        {
            var instance = new ConfiguredInstance(typeof (Service<>));
            family.SetDefault(instance);

            configuration.Default.ConcreteType.ShouldEqual(typeof (Service<>));
        }

        [Test]
        public void has_been_created_is_false()
        {
            configuration.As<IFamily>().HasBeenCreated(null).ShouldBeFalse();
        }

        [Test]
        public void has_implementations_is_false_if_there_are_no_instances_for_the_underlying_family()
        {
            configuration.HasImplementations().ShouldBeFalse();
        }

        [Test]
        public void has_implementations_is_true_if_there_are_instances_for_the_underlying_family()
        {
            var instance = new ConfiguredInstance(typeof (Service<>));
            family.SetDefault(instance);

            configuration.HasImplementations().ShouldBeTrue();
        }

        [Test]
        public void lifecycle_is_transient()
        {
            configuration.Lifecycle.ShouldBeOfType<TransientLifecycle>();
        }

        [Test]
        public void lifecyle_is_singleton()
        {
            family.SetScopeTo(Lifecycles.Singleton);
            configuration.Lifecycle.ShouldBeOfType<SingletonLifecycle>();
        }

        [Test]
        public void PluginType_pulls_from_the_inner_family()
        {
            configuration.PluginType.ShouldEqual(family.PluginType);
        }
    }
}