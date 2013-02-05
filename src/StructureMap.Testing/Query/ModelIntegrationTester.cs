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
        #region Setup/Teardown

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
                    o.Type<DefaultRule>();
                    o.Type<ARule>();
                    o.Type<ColorRule>().Ctor<string>("color").Is("red");
                });

                x.For<IEngine>().Use<PushrodEngine>();

                x.For<Startable1>().Singleton().Use<Startable1>();
                x.For<Startable2>().Use<Startable2>();
                x.For<Startable3>().Use<Startable3>();
            });
        }

        #endregion

        private Container container;

        [Test]
        public void can_iterate_through_families_including_both_generics_and_normal()
        {
            // +1 for "IContainer" itself + Func
            container.Model.PluginTypes.Count().ShouldEqual(9);

            container.Model.PluginTypes.Each(x => Debug.WriteLine(x.PluginType.FullName));
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
        public void can_iterate_through_instances_of_pipeline_graph_for_generics()
        {
            container.Model.For(typeof (IService<>)).Instances.Count().ShouldEqual(2);
        }

        [Test]
        public void can_iterate_through_instances_of_pipeline_graph_for_generics_from_model()
        {
            container.Model.InstancesOf(typeof (IService<>)).Count().ShouldEqual(2);
        }

        [Test]
        public void default_type_for_from_the_top()
        {
            container.Model.DefaultTypeFor<IWidget>().ShouldEqual(typeof (AWidget));
            container.Model.DefaultTypeFor<Rule>().ShouldBeNull();
        }

        [Test]
        public void get_all_instances_from_the_top()
        {
            container.Model.AllInstances.Count().ShouldEqual(12);
        }

        [Test]
        public void get_all_possibles()
        {
            // Startable1 is a singleton

            var startable1 = container.GetInstance<Startable1>();
            startable1.WasStarted.ShouldBeFalse();

            container.Model.GetAllPossible<IStartable>()
                .ToArray()
                .Each(x => x.Start())
                .Each(x => x.WasStarted.ShouldBeTrue());

            startable1.WasStarted.ShouldBeTrue();
        }

        [Test]
        public void has_default_implementation_from_the_top()
        {
            container.Model.HasDefaultImplementationFor<IWidget>().ShouldBeTrue();
            container.Model.HasDefaultImplementationFor<Rule>().ShouldBeFalse();
            container.Model.HasDefaultImplementationFor<IServiceProvider>().ShouldBeFalse();
        }

        [Test]
        public void has_implementation_from_the_top()
        {
            container.Model.HasDefaultImplementationFor<IServiceProvider>().ShouldBeFalse();
            container.Model.HasDefaultImplementationFor<IWidget>().ShouldBeTrue();
        }

        [Test]
        public void has_implementations_should_be_false_for_a_type_that_is_not_registered()
        {
            container.Model.For<ISomething>().HasImplementations().ShouldBeFalse();
        }

        [Test]
        public void remove_an_entire_closed_type()
        {
            container.Model.EjectAndRemove(typeof (Rule));
            container.Model.HasImplementationsFor<Rule>().ShouldBeFalse();

            container.TryGetInstance<Rule>().ShouldBeNull();
            container.GetAllInstances<Rule>().Count.ShouldEqual(0);
        }

        [Test]
        public void remove_an_entire_closed_type_with_the_filter()
        {
            container.Model.EjectAndRemovePluginTypes(t => t == typeof (Rule) || t == typeof (IWidget));

            container.Model.HasImplementationsFor<IWidget>().ShouldBeFalse();
            container.Model.HasImplementationsFor<Rule>().ShouldBeFalse();
            container.Model.HasImplementationsFor<IEngine>().ShouldBeTrue();
        }

        [Test]
        public void remove_an_open_type()
        {
            container.Model.EjectAndRemove(typeof (IService<>));

            container.Model.HasImplementationsFor(typeof (IService<>));

            container.TryGetInstance<IService<string>>().ShouldBeNull();
        }

        [Test]
        public void remove_an_open_type_with_a_filter()
        {
            container.Model.EjectAndRemovePluginTypes(t => t == typeof (IService<>));

            container.Model.HasImplementationsFor(typeof (IService<>));

            container.TryGetInstance<IService<string>>().ShouldBeNull();
        }

        [Test]
        public void remove_types_based_on_a_filter()
        {
            container.GetAllInstances<Rule>().Any(x => x is ARule).ShouldBeTrue();
            container.Model.HasImplementationsFor<IWidget>().ShouldBeTrue();

            container.Model.EjectAndRemoveTypes(t => t == typeof (IWidget) || t == typeof (ARule));

            container.GetAllInstances<Rule>().Any(x => x is ARule).ShouldBeFalse();
            container.Model.HasImplementationsFor<IWidget>().ShouldBeFalse();
        }
    }


    public interface IStartable
    {
        bool WasStarted { get; }
        void Start();
    }

    public class Startable : IStartable
    {
        public void Start()
        {
            WasStarted = true;
        }

        public bool WasStarted { get; private set; }
    }

    public class Startable1 : Startable
    {
    }

    public class Startable2 : Startable
    {
    }

    public class Startable3 : Startable
    {
    }
}