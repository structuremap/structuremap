using System;
using NUnit.Framework;
using Shouldly;
using StructureMap.Configuration.DSL;

namespace StructureMap.Testing.Bugs
{
    class container_configuration_with_generics_inheritance
    {
        class Base { }
        class Derived1 : Base { }
        class Derived2 : Derived1 { }
        class Derived3 : Derived2 { }
        class Derived4 : Derived3 { }
        class Derived5 : Derived4 { }
        class Derived6 : Derived5 { }

        private static bool CanResolve<T>(IContainer container)
        {
            return container.TryGetInstance(typeof(T)) != null;
        }

        [TestFixture]
        public class Register_generic_types_with_contravariance
        {
            [Test]
            public void configure_container_during_construction()
            {
                var container = new Container(new TestRegistry());
                AssertConfigurationIsCorrect(container);
            }

            [Test]
            public void configure_container_after_construction()
            {
                var container = new Container();
                container.Configure(x => x.IncludeRegistry(new TestRegistry()));
                AssertConfigurationIsCorrect(container);
            }

            private static void AssertConfigurationIsCorrect(Container container)
            {
                // can resolve explicitly registered generics
                container.GetInstance<IGenericContravariant<Derived1>>().ShouldBeOfType(typeof(GenericContravariant<Derived1>));
                container.GetInstance<IGenericContravariant<Derived3>>().ShouldBeOfType(typeof(GenericContravariant<Derived3>));
                container.GetInstance<IGenericContravariant<Derived5>>().ShouldBeOfType(typeof(GenericContravariant<Derived5>));

                // contravariance - can resolve generics with type param which inherits from an explicitly registered one
                CanResolve<IGenericContravariant<Base>>(container).ShouldBeFalse();
                CanResolve<IGenericContravariant<Derived2>>(container).ShouldBeTrue();
                CanResolve<IGenericContravariant<Derived4>>(container).ShouldBeTrue();
                CanResolve<IGenericContravariant<Derived6>>(container).ShouldBeTrue();
            }

            interface IGenericContravariant<in T> { }
            class GenericContravariant<T> : IGenericContravariant<T> { }

            class TestRegistry : Registry
            {
                public TestRegistry()
                {
                    // registration order:
                    // Derived1 before Derived5 - base class before inheriting class
                    // Derived5 before Derived3 - base class after inheriting class
                    For<IGenericContravariant<Derived1>>().Use<GenericContravariant<Derived1>>();
                    For<IGenericContravariant<Derived5>>().Use<GenericContravariant<Derived5>>();
                    For<IGenericContravariant<Derived3>>().Use<GenericContravariant<Derived3>>();
                }
            }
        }

        [TestFixture]
        public class Register_generic_types_with_covariance
        {
            [Test]
            public void configure_container_during_construction()
            {
                var container = new Container(new TestRegistry());
                AssertConfigurationIsCorrect(container);
            }

            [Test]
            public void configure_container_after_construction()
            {
                var container = new Container();
                container.Configure(x => x.IncludeRegistry(new TestRegistry()));
                AssertConfigurationIsCorrect(container);
            }

            private static void AssertConfigurationIsCorrect(Container container)
            {
                // can resolve explicitly registered generics
                container.GetInstance<IGenericCovariant<Derived1>>().ShouldBeOfType(typeof(GenericCovariant<Derived1>));
                container.GetInstance<IGenericCovariant<Derived3>>().ShouldBeOfType(typeof(GenericCovariant<Derived3>));
                container.GetInstance<IGenericCovariant<Derived5>>().ShouldBeOfType(typeof(GenericCovariant<Derived5>));

                // covariance - can resolve generics with type param which is base of an explicitly registered one
                CanResolve<IGenericCovariant<Base>>(container).ShouldBeTrue();
                CanResolve<IGenericCovariant<Derived2>>(container).ShouldBeTrue();
                CanResolve<IGenericCovariant<Derived4>>(container).ShouldBeTrue();
                CanResolve<IGenericCovariant<Derived6>>(container).ShouldBeFalse();
            }

            interface IGenericCovariant<out T> { }
            class GenericCovariant<T> : IGenericCovariant<T> { }

            class TestRegistry : Registry
            {
                public TestRegistry()
                {
                    // registration order:
                    // Derived1 before Derived5 - base class before inheriting class
                    // Derived5 before Derived3 - base class after inheriting class
                    For<IGenericCovariant<Derived1>>().Use<GenericCovariant<Derived1>>();
                    For<IGenericCovariant<Derived5>>().Use<GenericCovariant<Derived5>>();
                    For<IGenericCovariant<Derived3>>().Use<GenericCovariant<Derived3>>();
                }
            }
        }

        [TestFixture]
        public class Register_generic_types_with_no_variance
        {
            [Test]
            public void configure_container_during_construction()
            {
                var container = new Container(new TestRegistry());
                AssertConfigurationIsCorrect(container);
            }

            [Test]
            public void configure_container_after_construction()
            {
                var container = new Container();
                container.Configure(x => x.IncludeRegistry(new TestRegistry()));
                AssertConfigurationIsCorrect(container);
            }

            private static void AssertConfigurationIsCorrect(Container container)
            {
                // can resolve explicitly registered generics
                container.GetInstance<IGeneric<Derived1>>().ShouldBeOfType(typeof(Generic<Derived1>));
                container.GetInstance<IGeneric<Derived3>>().ShouldBeOfType(typeof(Generic<Derived3>));
                container.GetInstance<IGeneric<Derived5>>().ShouldBeOfType(typeof(Generic<Derived5>));

                // no variance - can't resolve generics with base or inheriting classes
                CanResolve<IGeneric<Base>>(container).ShouldBeFalse();
                CanResolve<IGeneric<Derived2>>(container).ShouldBeFalse();
                CanResolve<IGeneric<Derived4>>(container).ShouldBeFalse();
                CanResolve<IGeneric<Derived6>>(container).ShouldBeFalse();
            }

            interface IGeneric<T> { }
            class Generic<T> : IGeneric<T> { }

            class TestRegistry : Registry
            {
                public TestRegistry()
                {
                    // registration order:
                    // Derived1 before Derived5 - base class before inheriting class
                    // Derived5 before Derived3 - base class after inheriting class
                    For<IGeneric<Derived1>>().Use<Generic<Derived1>>();
                    For<IGeneric<Derived5>>().Use<Generic<Derived5>>();
                    For<IGeneric<Derived3>>().Use<Generic<Derived3>>();
                }
            }
        }

        [TestFixture]
        public class Register_generic_types_with_inheritance
        {
            [Test]
            public void configure_container_during_construction()
            {
                var container = new Container(new TestRegistry());
                AssertConfigurationIsCorrect(container);
            }

            [Test]
            public void configure_container_after_construction()
            {
                var container = new Container();
                container.Configure(x => x.IncludeRegistry(new TestRegistry()));
                AssertConfigurationIsCorrect(container);
            }

            private static void AssertConfigurationIsCorrect(Container container)
            {
                // can resolve explicitly registered generics
                container.GetInstance<IGeneric<string>>().ShouldBeOfType(typeof(GenericImpl<string>));
                container.GetInstance<IGeneric<string>>().Value.ShouldBe("base");
                container.GetInstance<IMoreSpecificGeneric<string>>().ShouldBeOfType(typeof(GenericImpl<string>));
                container.GetInstance<IMoreSpecificGeneric<string>>().Value.ShouldBe("derived");
                container.GetInstance<IMostSpecificGeneric<string>>().ShouldBeOfType(typeof(GenericImpl<string>));
                container.GetInstance<IMostSpecificGeneric<string>>().Value.ShouldBe("most derived");
            }

            interface IGeneric<T>
            {
                T Value { get; }
            }
            interface IMoreSpecificGeneric<T> : IGeneric<T> { }
            interface IMostSpecificGeneric<T> : IMoreSpecificGeneric<T> { }

            class GenericImpl<T> : IMostSpecificGeneric<string>
            {
                public string Value { get; set; }
            }

            class TestRegistry : Registry
            {
                public TestRegistry()
                {
                    // registration order:
                    // IGeneric before IMostSpecificGeneric - base class before inheriting class
                    // IMostSpecificGeneric before IMoreSpecificGeneric - base class after inheriting class
                    For<IGeneric<string>>().Use(new GenericImpl<string> { Value = "base" });
                    For<IMostSpecificGeneric<string>>().Use(new GenericImpl<string> { Value = "most derived" });
                    For<IMoreSpecificGeneric<string>>().Use(new GenericImpl<string> { Value = "derived" });
                }
            }
        }
    }
}
