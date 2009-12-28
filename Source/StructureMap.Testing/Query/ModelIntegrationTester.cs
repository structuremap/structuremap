using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using StructureMap.Testing.Configuration.DSL;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Graph;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget2;

namespace StructureMap.Testing.Query
{
    [TestFixture]
    public class ModelIntegrationTester
    {
        private Container container;

        [SetUp]
        public void SetUp()
        {
            container = new Container(x =>
            {
                x.For(typeof (IService<>)).Add(typeof (Service<>));
                x.For(typeof (IService<>)).Add(typeof (Service2<>));

                x.For<IWidget>().Singleton().Use<AWidget>();
                x.For<Rule>().AddInstances(o =>
                {
                    o.OfConcreteType<DefaultRule>();
                    o.OfConcreteType<ARule>();
                    o.OfConcreteType<ColorRule>().WithCtorArg("color").EqualTo("red");
                });

                x.For<IEngine>().Use<PushrodEngine>();
            });
        }

        [Test]
        public void get_all_instances_from_the_top()
        {
            container.Model.AllInstances.Count().ShouldEqual(8);
        }

        [Test]
        public void default_type_for_from_the_top()
        {
            container.Model.DefaultTypeFor<IWidget>().ShouldEqual(typeof (AWidget));
            container.Model.DefaultTypeFor<Rule>().ShouldBeNull();
        }

        [Test]
        public void has_implementation_from_the_top()
        {
            container.Model.HasDefaultImplementationFor<IServiceProvider>().ShouldBeFalse();
            container.Model.HasDefaultImplementationFor<IWidget>().ShouldBeTrue();
        }

        [Test]
        public void has_default_implementation_from_the_top()
        {
            container.Model.HasDefaultImplementationFor<IWidget>().ShouldBeTrue();
            container.Model.HasDefaultImplementationFor<Rule>().ShouldBeFalse();
            container.Model.HasDefaultImplementationFor<IServiceProvider>().ShouldBeFalse();
        }

        [Test]
        public void can_iterate_through_families_including_both_generics_and_normal()
        {
            // +1 for "IContainer" itself
            container.Model.PluginTypes.Count().ShouldEqual(5);

            container.Model.PluginTypes.Each(x => Debug.WriteLine(x.PluginType.FullName));
        }

        [Test]
        public void can_iterate_through_instances_of_pipeline_graph_for_generics()
        {
            container.Model.For(typeof (IService<>)).Instances.Count().ShouldEqual(2);
        }

        [Test]
        public void can_iterate_through_instances_of_pipeline_graph_for_generics_from_model()
        {
            container.Model.InstancesOf(typeof(IService<>)).Count().ShouldEqual(2);
        }

        [Test]
        public void can_iterate_through_instances_of_pipeline_graph_for_closed_type_from_model()
        {
            container.Model.InstancesOf<Rule>().Count().ShouldEqual(3);
        }

        [Test]
        public void can_iterate_through_instances_of_pipeline_graph_for_closed_type_that_is_not_registered()
        {
            container.Model.InstancesOf<IServiceProvider>().Count().ShouldEqual(0);
        }

        [Test]
        public void has_implementations_should_be_false_for_a_type_that_is_not_registered()
        {
            container.Model.For<ISomething>().HasImplementations().ShouldBeFalse();
        }
    }
}