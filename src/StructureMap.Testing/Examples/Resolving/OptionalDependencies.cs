using NUnit.Framework;
using StructureMap.Testing.GenericWidgets;

namespace StructureMap.Testing.Examples.Resolving
{
    [TestFixture]
    public class OptionalDependencies
    {
        // SAMPLE: optional-foo
        public interface IFoo{}
        public class Foo : IFoo{}
        // ENDSAMPLE

        // SAMPLE: optional-got-it
        [Test]
        public void i_have_got_that()
        {
            var container = new Container(_ => _.For<IFoo>().Use<Foo>());

            container.TryGetInstance<IFoo>()
                .ShouldNotBeNull();
        
            // -- or --

            container.TryGetInstance(typeof(IFoo))
                .ShouldNotBeNull();
        }
        // ENDSAMPLE

        // SAMPLE: optional-dont-got-it
        [Test]
        public void i_do_not_have_that()
        {
            var container = new Container();

            container.TryGetInstance<IFoo>()
                .ShouldBeNull();

            // -- or --

            container.TryGetInstance(typeof(IFoo))
                .ShouldBeNull();
        }
        // ENDSAMPLE

        // SAMPLE: optional-no-concrete
        public class ConcreteThing{}

        [Test]
        public void no_auto_resolution_of_concrete_types()
        {
            var container = new Container();

            container.TryGetInstance<ConcreteThing>()
                .ShouldBeNull();

            // now register ConcreteThing and do it again
            container.Configure(_ => {
                _.For<ConcreteThing>().Use<ConcreteThing>();
            });

            container.TryGetInstance<ConcreteThing>()
                .ShouldNotBeNull();
        }
        // ENDSAMPLE

        // SAMPLE: optional-close-generics
        public interface IThing<T>{}
        public class Thing<T> : IThing<T>{}

        [Test]
        public void can_try_get_open_type_resolution()
        {
            var container = new Container(_ => {
                _.For(typeof (IThing<>)).Use(typeof (Thing<>));
            });

            container.TryGetInstance<IThing<string>>()
                .ShouldBeOfType<Thing<string>>();
        }
        // ENDSAMPLE

        // SAMPLE: optional-real-usage
        public class MyFoo : IFoo{}

        [Test]
        public void real_usage()
        {
            var container = new Container();

            // if the container doesn't know about it,
            // I'll build it myself
            var foo = container.TryGetInstance<IFoo>()
                      ?? new MyFoo();
        }
        // ENDSAMPLE
    }
}