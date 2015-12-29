using NUnit.Framework;
using Shouldly;
using StructureMap.Configuration.DSL;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Register_generic_types_with_contravariance
    {
        [Test]
        public void configure_container_during_construction()
        {
            var container = new Container(new TestRegistry());
            
            container.GetInstance<IGenericContravariant<Foo>>().ShouldBeOfType(typeof(GenericContravariantOfFoo));
            container.GetInstance<IGenericContravariant<Bar>>().ShouldBeOfType(typeof(GenericContravariantOfBar));
        }

        [Test]
        public void configure_container_after_construction()
        {
            var container = new Container();
            container.Configure(x => x.IncludeRegistry(new TestRegistry()));
            
            container.GetInstance<IGenericContravariant<Foo>>().ShouldBeOfType(typeof(GenericContravariantOfFoo));
            container.GetInstance<IGenericContravariant<Bar>>().ShouldBeOfType(typeof(GenericContravariantOfBar));
        }

        interface IGenericContravariant<in T> { }
        class GenericContravariantOfFoo : IGenericContravariant<Foo> { }
        class GenericContravariantOfBar : IGenericContravariant<Bar> { }

        class Foo { }
        class Bar : Foo { }

        class TestRegistry : Registry
        {
            public TestRegistry()
            {
                For<IGenericContravariant<Foo>>().Use<GenericContravariantOfFoo>();
                For<IGenericContravariant<Bar>>().Use<GenericContravariantOfBar>();
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

            container.GetInstance<IGenericCovariant<Foo>>().ShouldBeOfType(typeof(GenericCovariantOfFoo));
            container.GetInstance<IGenericCovariant<Bar>>().ShouldBeOfType(typeof(GenericCovariantOfBar));
        }

        [Test]
        public void configure_container_after_construction()
        {
            var container = new Container();
            container.Configure(x => x.IncludeRegistry(new TestRegistry()));

            container.GetInstance<IGenericCovariant<Foo>>().ShouldBeOfType(typeof(GenericCovariantOfFoo));
            container.GetInstance<IGenericCovariant<Bar>>().ShouldBeOfType(typeof(GenericCovariantOfBar));
        }

        interface IGenericCovariant<out T> { }
        class GenericCovariantOfFoo : IGenericCovariant<Foo> { }
        class GenericCovariantOfBar : IGenericCovariant<Bar> { }

        class Foo { }
        class Bar : Foo { }

        class TestRegistry : Registry
        {
            public TestRegistry()
            {
                For<IGenericCovariant<Foo>>().Use<GenericCovariantOfFoo>();
                For<IGenericCovariant<Bar>>().Use<GenericCovariantOfBar>();
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

            container.GetInstance<IGeneric<Foo>>().ShouldBeOfType(typeof(GenericOfFoo));
            container.GetInstance<IGeneric<Bar>>().ShouldBeOfType(typeof(GenericOfBar));
        }

        [Test]
        public void configure_container_after_construction()
        {
            var container = new Container();
            container.Configure(x => x.IncludeRegistry(new TestRegistry()));

            container.GetInstance<IGeneric<Foo>>().ShouldBeOfType(typeof(GenericOfFoo));
            container.GetInstance<IGeneric<Bar>>().ShouldBeOfType(typeof(GenericOfBar));
        }

        interface IGeneric<T> { }
        class GenericOfFoo : IGeneric<Foo> { }
        class GenericOfBar : IGeneric<Bar> { }

        class Foo { }
        class Bar : Foo { }

        class TestRegistry : Registry
        {
            public TestRegistry()
            {
                For<IGeneric<Foo>>().Use<GenericOfFoo>();
                For<IGeneric<Bar>>().Use<GenericOfBar>();
            }
        }
    }
}
