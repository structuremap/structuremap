using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Configuration.DSL;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class NestedContainerSupportTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void the_nested_container_delivers_itself_as_the_IContainer()
        {
            var parent = new Container(x =>
            {
                x.For<IWidget>().Use<AWidget>();
            });

            var child = parent.GetNestedContainer();

            child.GetInstance<IContainer>().ShouldBeTheSameAs(child);
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

            using (var nested = container.GetNestedContainer())
            {
                nested.GetAllInstances<IBar>().Count.ShouldBeGreaterThan(0);
            }

            container.GetAllInstances<IBar>().Count.ShouldBeGreaterThan(0);
        }

        public interface IBar{}
        public class AFoo : IBar{}
        public class BFoo : IBar{}
        public class CFoo : IBar{}

        [Test]
        public void transient_service_in_the_parent_container_is_effectively_a_singleton_for_the_nested_container()
        {
            var parent = new Container(x =>
            {
                x.For<IWidget>().Use<AWidget>();
            });

            var child = parent.GetNestedContainer();

            var childWidget1 = child.GetInstance<IWidget>();
            var childWidget2 = child.GetInstance<IWidget>();
            var childWidget3 = child.GetInstance<IWidget>();

            var parentWidget = parent.GetInstance<IWidget>();

            childWidget1.ShouldBeTheSameAs(childWidget2);
            childWidget1.ShouldBeTheSameAs(childWidget3);
            childWidget1.ShouldNotBeTheSameAs(parentWidget);
        }

        [Test]
        public void inject_into_the_child_does_not_affect_the_parent_container()
        {
            var parent = new Container(x =>
            {
                x.For<IWidget>().Use<AWidget>();
            });

            var child = parent.GetNestedContainer();
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
        public void singleton_service_in_the_parent_is_found_by_the_child()
        {
            var parent = new Container(x =>
            {
                x.ForSingletonOf<IWidget>().Use<AWidget>();
            });

            var parentWidget = parent.GetInstance<IWidget>();

            var child = parent.GetNestedContainer();

            var childWidget1 = child.GetInstance<IWidget>();
            var childWidget2 = child.GetInstance<IWidget>();

            parentWidget.ShouldBeTheSameAs(childWidget1);
            parentWidget.ShouldBeTheSameAs(childWidget2);
        }

        [Test]
        public void transient_open_generics_service_in_the_parent_container_is_effectively_a_singleton_for_the_nested_container()
        {
            var parent = new Container(x =>
            {
                x.For(typeof(IService<>)).Use(typeof(Service<>));
            });

            var child = parent.GetNestedContainer();

            var childWidget1 = child.GetInstance<IService<string>>();
            var childWidget2 = child.GetInstance<IService<string>>();
            var childWidget3 = child.GetInstance<IService<string>>();

            var parentWidget = parent.GetInstance<IService<string>>();

            childWidget1.ShouldBeTheSameAs(childWidget2);
            childWidget1.ShouldBeTheSameAs(childWidget3);
            childWidget1.ShouldNotBeTheSameAs(parentWidget);
        }

        [Test]
        public void singleton_service_from_open_type_in_the_parent_is_found_by_the_child()
        {
            var parent = new Container(x =>
            {
                x.For(typeof(IService<>)).CacheBy(InstanceScope.Singleton).Use(typeof(Service<>));
            });

            var child = parent.GetNestedContainer();

            var childWidget1 = child.GetInstance<IService<string>>();
            var childWidget2 = child.GetInstance<IService<string>>();
            var childWidget3 = child.GetInstance<IService<string>>();

            var parentWidget = parent.GetInstance<IService<string>>();

            childWidget1.ShouldBeTheSameAs(childWidget2);
            childWidget1.ShouldBeTheSameAs(childWidget3);
            childWidget1.ShouldBeTheSameAs(parentWidget);
        }

        [Test]
        public void get_a_nested_container_for_a_profile()
        {
            var parent = new Container(x =>
            {
                x.For<IWidget>().TheDefault.Is.OfConcreteType<ColorWidget>()
                    .WithCtorArg("color").EqualTo("red");

                x.CreateProfile("green", o =>
                {
                    o.Type<IWidget>().Is.OfConcreteType<ColorWidget>()
                        .WithCtorArg("color").EqualTo("green");
                });

            });

            var child = parent.GetNestedContainer("green");

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
    }
}