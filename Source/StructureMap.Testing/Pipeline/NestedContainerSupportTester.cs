using NUnit.Framework;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Graph;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class NestedContainerSupportTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        public class ContainerHolder
        {
            private readonly IContainer _container;

            public ContainerHolder(IContainer container)
            {
                _container = container;
            }

            public IContainer Container { get { return _container; } }
        }

        public interface IBar
        {
        }

        public class AFoo : IBar
        {
        }

        public class BFoo : IBar
        {
        }

        public class CFoo : IBar
        {
        }

        [Test]
        public void allow_nested_container_to_report_what_it_has()
        {
            var container = new Container(x => x.For<IAutomobile>().Use<Mustang>());

            IContainer nestedContainer = container.GetNestedContainer();
            nestedContainer.Inject<IEngine>(new PushrodEngine());

            container.WhatDoIHave().ShouldNotBeEmpty().ShouldNotContain(typeof (IEngine).Name);
            nestedContainer.WhatDoIHave().ShouldNotBeEmpty().ShouldContain(typeof (IEngine).Name);
        }

        [Test]
        public void disposing_the_child_container_does_not_affect_the_parent_container()
        {
            var container = new Container(x =>
            {
                x.Scan(o =>
                {
                    o.TheCallingAssembly();
                    o.AddAllTypesOf<IBar>();
                });
            });

            container.GetAllInstances<IBar>().Count.ShouldBeGreaterThan(0);

            using (IContainer nested = container.GetNestedContainer())
            {
                nested.GetAllInstances<IBar>().Count.ShouldBeGreaterThan(0);
            }

            container.GetAllInstances<IBar>().Count.ShouldBeGreaterThan(0);
        }

        [Test]
        public void get_a_nested_container_for_a_profile()
        {
            var parent = new Container(x =>
            {
                x.For<IWidget>().Use<ColorWidget>()
                    .WithCtorArg("color").EqualTo("red");

                x.Profile("green", o =>
                {
                    o.Type<IWidget>().Is.OfConcreteType<ColorWidget>()
                        .WithCtorArg("color").EqualTo("green");
                });
            });

            IContainer child = parent.GetNestedContainer("green");

            var childWidget1 = child.GetInstance<IWidget>();
            var childWidget2 = child.GetInstance<IWidget>();
            var childWidget3 = child.GetInstance<IWidget>();

            var parentWidget = parent.GetInstance<IWidget>();

            childWidget1.ShouldBeTheSameAs(childWidget2);
            childWidget1.ShouldBeTheSameAs(childWidget3);
            childWidget1.ShouldNotBeTheSameAs(parentWidget);

            parentWidget.ShouldBeOfType<ColorWidget>().Color.ShouldEqual("red");
            childWidget1.ShouldBeOfType<ColorWidget>().Color.ShouldEqual("green");
        }

        [Test]
        public void inject_into_the_child_does_not_affect_the_parent_container()
        {
            var parent = new Container(x => { x.For<IWidget>().Use<AWidget>(); });

            IContainer child = parent.GetNestedContainer();
            var childWidget = new ColorWidget("blue");
            child.Inject<IWidget>(childWidget);

            // do the check repeatedly
            child.GetInstance<IWidget>().ShouldBeTheSameAs(childWidget);
            child.GetInstance<IWidget>().ShouldBeTheSameAs(childWidget);
            child.GetInstance<IWidget>().ShouldBeTheSameAs(childWidget);


            // now, compare to the parent
            parent.GetInstance<IWidget>().ShouldNotBeTheSameAs(childWidget);
        }

        [Test]
        public void singleton_service_from_open_type_in_the_parent_is_found_by_the_child()
        {
            var parent =
                new Container(
                    x => { x.For(typeof (IService<>)).CacheBy(InstanceScope.Singleton).Use(typeof (Service<>)); });

            IContainer child = parent.GetNestedContainer();

            var childWidget1 = child.GetInstance<IService<string>>();
            var childWidget2 = child.GetInstance<IService<string>>();
            var childWidget3 = child.GetInstance<IService<string>>();

            var parentWidget = parent.GetInstance<IService<string>>();

            childWidget1.ShouldBeTheSameAs(childWidget2);
            childWidget1.ShouldBeTheSameAs(childWidget3);
            childWidget1.ShouldBeTheSameAs(parentWidget);
        }

        [Test]
        public void singleton_service_in_the_parent_is_found_by_the_child()
        {
            var parent = new Container(x => { x.ForSingletonOf<IWidget>().Use<AWidget>(); });

            var parentWidget = parent.GetInstance<IWidget>();

            IContainer child = parent.GetNestedContainer();

            var childWidget1 = child.GetInstance<IWidget>();
            var childWidget2 = child.GetInstance<IWidget>();

            parentWidget.ShouldBeTheSameAs(childWidget1);
            parentWidget.ShouldBeTheSameAs(childWidget2);
        }

        [Test]
        public void the_nested_container_delivers_itself_as_the_IContainer()
        {
            var parent = new Container(x => { x.For<IWidget>().Use<AWidget>(); });

            IContainer child = parent.GetNestedContainer();

            child.GetInstance<IContainer>().ShouldBeTheSameAs(child);
        }

        [Test]
        public void the_nested_container_will_deliver_itself_into_a_constructor_of_something_else()
        {
            var parent = new Container(x => { x.For<IWidget>().Use<AWidget>(); });

            IContainer child = parent.GetNestedContainer();
            child.GetInstance<ContainerHolder>().Container.ShouldBeTheSameAs(child);
        }

        [Test]
        public void
            transient_open_generics_service_in_the_parent_container_is_effectively_a_singleton_for_the_nested_container()
        {
            var parent = new Container(x => { x.For(typeof (IService<>)).Use(typeof (Service<>)); });

            IContainer child = parent.GetNestedContainer();

            var childWidget1 = child.GetInstance<IService<string>>();
            var childWidget2 = child.GetInstance<IService<string>>();
            var childWidget3 = child.GetInstance<IService<string>>();

            var parentWidget = parent.GetInstance<IService<string>>();

            childWidget1.ShouldBeTheSameAs(childWidget2);
            childWidget1.ShouldBeTheSameAs(childWidget3);
            childWidget1.ShouldNotBeTheSameAs(parentWidget);
        }

        [Test]
        public void transient_service_in_the_parent_container_is_effectively_a_singleton_for_the_nested_container()
        {
            var parent = new Container(x =>
            {
                // IWidget is a "transient"
                x.For<IWidget>().Use<AWidget>();
            });

            IContainer child = parent.GetNestedContainer();

            var childWidget1 = child.GetInstance<IWidget>();
            var childWidget2 = child.GetInstance<IWidget>();
            var childWidget3 = child.GetInstance<IWidget>();

            var parentWidget = parent.GetInstance<IWidget>();

            childWidget1.ShouldBeTheSameAs(childWidget2);
            childWidget1.ShouldBeTheSameAs(childWidget3);
            childWidget1.ShouldNotBeTheSameAs(parentWidget);
        }
    }
}