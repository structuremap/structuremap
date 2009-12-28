using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Testing.Configuration.DSL;
using StructureMap.Testing.Graph;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget2;
using System.Linq;

namespace StructureMap.Testing.Query
{
    [TestFixture]
    public class InstanceFactoryTypeConfigurationTester
    {
        private Container container;

        [SetUp]
        public void SetUp()
        {
            container = new Container(x =>
            {
                x.For<IWidget>().Singleton().Use<AWidget>();
                x.For<Rule>().AddInstances(o =>
                {
                    o.OfConcreteType<DefaultRule>();
                    o.OfConcreteType<ARule>();
                    o.OfConcreteType<ColorRule>().WithCtorArg("color").EqualTo("red");
                });

                x.For<IEngine>().Use<PushrodEngine>();

                x.For<IAutomobile>();
            });
        }

        [Test]
        public void has_implementations_positive_test()
        {
            container.Model.For<Rule>().HasImplementations().ShouldBeTrue();
            container.Model.For<IWidget>().HasImplementations().ShouldBeTrue();
        }

        [Test]
        public void has_implementations_negative_test()
        {
            container.Model.For<IAutomobile>().HasImplementations().ShouldBeFalse();
        }

        [Test]
        public void get_lifecycle()
        {
            container.Model.For<IWidget>().Lifecycle.ShouldEqual(InstanceScope.Singleton.ToString());
            container.Model.For<Rule>().Lifecycle.ShouldEqual(InstanceScope.Transient.ToString());
        }

        [Test]
        public void build_when_the_cast_does_not_work()
        {
            container.Model.For<IWidget>().Default.Get<Rule>().ShouldBeNull();
        }

        [Test]
        public void build_when_the_cast_does_work()
        {
            container.Model.For<IWidget>().Default.Get<IWidget>().ShouldBeOfType<AWidget>();
        }

        [Test]
        public void building_respects_the_lifecycle()
        {
            var widget1 = container.Model.For<IWidget>().Default.Get<IWidget>();
            var widget2 = container.Model.For<IWidget>().Default.Get<IWidget>();

            widget1.ShouldBeTheSameAs(widget2);
        }

        [Test]
        public void get_default_when_it_exists()
        {
            container.Model.For<IWidget>().Default.ConcreteType.ShouldEqual(typeof (AWidget));
        }

        [Test]
        public void get_default_should_return_null_when_it_does_not_exist()
        {
            container.Model.For<Rule>().Default.ShouldBeNull();
        }

        [Test]
        public void has_been_created_for_a_singleton()
        {
            container.Model.For<IWidget>().Default.ObjectHasBeenCreated().ShouldBeFalse();
            container.GetInstance<IWidget>();
            container.Model.For<IWidget>().Default.ObjectHasBeenCreated().ShouldBeTrue();
        }

        [Test]
        public void has_been_created_for_a_purely_transient_object_should_always_be_false()
        {
            container.Model.For<IEngine>().Default.ObjectHasBeenCreated().ShouldBeFalse();

            container.GetInstance<IEngine>();

            container.Model.For<IEngine>().Default.ObjectHasBeenCreated().ShouldBeFalse();
        }


        [Test]
        public void has_been_created_for_a_transient_type_in_a_container_should_be_tracked()
        {
            var nested = container.GetNestedContainer();

            nested.Model.For<IEngine>().Default.ObjectHasBeenCreated().ShouldBeFalse();

            nested.GetInstance<IEngine>();

            nested.Model.For<IEngine>().Default.ObjectHasBeenCreated().ShouldBeTrue();
        }

        [Test]
        public void eject_a_singleton()
        {
            var widget1 = container.GetInstance<IWidget>();
            container.GetInstance<IWidget>().ShouldBeTheSameAs(widget1);

            container.Model.For<IWidget>().Default.EjectObject();

            container.GetInstance<IWidget>().ShouldNotBeTheSameAs(widget1);
        }

        [Test]
        public void eject_a_singleton_that_has_not_been_created_does_no_harm()
        {
            container.Model.For<IWidget>().Default.EjectObject();
        }

        [Test]
        public void eject_a_transient_does_no_harm()
        {
            container.Model.For<IEngine>().Default.EjectObject();
        }

        [Test]
        public void can_iterate_over_the_children_instances()
        {
            container.Model.InstancesOf<Rule>().Count().ShouldEqual(3);
        }

        
    }
}