using System.Linq;
using NUnit.Framework;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class full_stack_container_usage
    {
        public interface IFoo
        {
        }

        public class AFoo : IFoo
        {
        }

        public class BFoo : IFoo
        {
        }

        public class CFoo : IFoo
        {
        }

        [Test]
        public void builds_all_instances_from_get_all()
        {
            var container = new Container(x => {
                x.For<IFoo>().AddInstances(o => {
                    o.Type<AFoo>();
                    o.Type<BFoo>();
                    o.Type<CFoo>();
                });
            });

            container.GetAllInstances<IFoo>()
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof (AFoo), typeof (BFoo), typeof (CFoo));
        }

        // SAMPLE: container-configure
        [Test]
        public void change_default_in_an_existing_container()
        {
            var container = new Container(x => { x.For<IFoo>().Use<AFoo>(); });

            container.GetInstance<IFoo>().ShouldBeOfType<AFoo>();

            // Now, change the container configuration
            container.Configure(x => x.For<IFoo>().Use<BFoo>());

            // The default of IFoo is now different
            container.GetInstance<IFoo>().ShouldBeOfType<BFoo>();

            // or use the Inject method that's just syntactical
            // sugar for replacing the default of one type at a time

            container.Inject<IFoo>(new CFoo());

            container.GetInstance<IFoo>().ShouldBeOfType<CFoo>();
        }
        // ENDSAMPLE


        public interface IOpen<T>
        {
        }

        public class AOpen<T> : IOpen<T>
        {
        }

        public class BOpen<T> : IOpen<T>
        {
        }

        [Test]
        public void change_default_of_generic_method()
        {
            var container = new Container(x => { x.For(typeof (IOpen<>)).Use(typeof (AOpen<>)); });

            container.Configure(x => { x.For(typeof (IOpen<>)).Use(typeof (BOpen<>)); });

            container.GetInstance<IOpen<string>>()
                .ShouldBeOfType<BOpen<string>>();
        }


        public class GuyWithOpenAndFoo
        {
            public IFoo Foo { get; set; }
            public IOpen<string> Open { get; set; }

            public GuyWithOpenAndFoo(IFoo foo, IOpen<string> open)
            {
                Foo = foo;
                Open = open;
            }
        }

        [Test]
        public void auto_resolve_a_concrete_type_with_defaults()
        {
            var container = new Container(x => {
                x.For<IFoo>().Use<CFoo>();
                x.For(typeof (IOpen<>)).Use(typeof (BOpen<>));
            });

            var guy = container.GetInstance<GuyWithOpenAndFoo>();
            guy.Foo.ShouldBeOfType<CFoo>();
            guy.Open.ShouldBeOfType<BOpen<string>>();
        }

        [Test]
        public void auto_resolve_a_concrete_type_with_defaults_with_the_type()
        {
            var container = new Container(x => {
                x.For<IFoo>().Use<CFoo>();
                x.For(typeof (IOpen<>)).Use(typeof (BOpen<>));
            });

            var guy = container.GetInstance(typeof (GuyWithOpenAndFoo))
                .ShouldBeOfType<GuyWithOpenAndFoo>();
            guy.Foo.ShouldBeOfType<CFoo>();
            guy.Open.ShouldBeOfType<BOpen<string>>();
        }


        public class GrandChild
        {
            private readonly string _name;

            public GrandChild(string name)
            {
                _name = name;
            }

            public string Name
            {
                get { return _name; }
            }
        }

        public class Child
        {
            private readonly string _name;
            private readonly GrandChild _grandChild;

            public Child(string name, GrandChild grandChild)
            {
                _name = name;
                _grandChild = grandChild;
            }

            public string Name
            {
                get { return _name; }
            }

            public GrandChild GrandChild
            {
                get { return _grandChild; }
            }
        }

        public interface IParent
        {
            string Name { get; }
            Child Child { get; }
        }

        public class Parent : IParent
        {
            private readonly string _name;
            private readonly Child _child;

            public Parent(string name, Child child)
            {
                _name = name;
                _child = child;
            }

            public string Name
            {
                get { return _name; }
            }

            public Child Child
            {
                get { return _child; }
            }
        }

        [Test]
        public void deep_graph()
        {
            var container = new Container(x => {
                x.For<IParent>().Use<Parent>()
                    .Ctor<string>("name").Is("Jerry")
                    .Ctor<Child>().Is<Child>(child => {
                        child.Ctor<string>("name").Is("Monte")
                            .Ctor<GrandChild>().Is<GrandChild>(grand => { grand.Ctor<string>("name").Is("Jeremy"); });
                    });
            });

            var parent = container.GetInstance<IParent>()
                .ShouldBeOfType<Parent>();

            parent.Name.ShouldEqual("Jerry");
            parent.Child.Name.ShouldEqual("Monte");
            parent.Child.GrandChild.Name.ShouldEqual("Jeremy");
        }


        [Test]
        public void If_there_is_only_one_instance_of_a_type_use_that_as_default()
        {
            var target = new AFoo();

            var container = new Container(registry => registry.For<IFoo>().Use(target));

            container.GetInstance<IFoo>()
                .ShouldBeTheSameAs(target);
        }

        [Test]
        public void get_by_name()
        {
            var container = new Container(x => {
                x.For<IFoo>().Add<AFoo>().Named("A");
                x.For<IFoo>().Add<BFoo>().Named("B");
                x.For<IFoo>().Add<CFoo>().Named("C");
            });

            container.GetInstance<IFoo>("A").ShouldBeOfType<AFoo>();
            container.GetInstance<IFoo>("B").ShouldBeOfType<BFoo>();
            container.GetInstance<IFoo>("C").ShouldBeOfType<CFoo>();
        }
    }
}