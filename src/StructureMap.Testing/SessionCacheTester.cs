using System;
using System.Diagnostics;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Building;
using StructureMap.Pipeline;

namespace StructureMap.Testing
{
    [TestFixture]
    public class SessionCacheTester
    {
        private IBuildSession theResolver;
        private SessionCache theCache;
        private IPipelineGraph thePipeline;

        [SetUp]
        public void SetUp()
        {
            theResolver = MockRepository.GenerateMock<IBuildSession>();
            theCache = new SessionCache(theResolver);

            thePipeline = MockRepository.GenerateMock<IPipelineGraph>();
        }

        [Test]
        public void get_instance_if_the_object_does_not_already_exist()
        {
            var instance = new ConfiguredInstance(typeof (Foo));

            var foo = new Foo();

            theResolver.Stub(x => x.ResolveFromLifecycle(typeof (IFoo), instance)).Return(foo);


            theCache.GetObject(typeof (IFoo), instance).ShouldBeTheSameAs(foo);
        }

        [Test]
        public void get_instance_if_the_object_is_unique_and_does_not_exist()
        {
            var instance = new ConfiguredInstance(typeof (Foo));
            instance.SetScopeTo(Lifecycles.Unique);

            var foo = new Foo();
            var foo2 = new Foo();

            theResolver.Stub(x => x.BuildNewInSession(typeof (IFoo), instance)).Return(foo).Repeat.Once();
            theResolver.Stub(x => x.BuildNewInSession(typeof (IFoo), instance)).Return(foo2).Repeat.Once();

            theCache.GetObject(typeof (IFoo), instance).ShouldBeTheSameAs(foo);
            theCache.GetObject(typeof (IFoo), instance).ShouldBeTheSameAs(foo2);
        }

        [Test]
        public void get_instance_remembers_the_first_object_created()
        {
            var instance = new ConfiguredInstance(typeof (Foo));

            var foo = new Foo();

            theResolver.Expect(x => x.ResolveFromLifecycle(typeof (IFoo), instance)).Return(foo).Repeat.Once();


            theCache.GetObject(typeof (IFoo), instance).ShouldBeTheSameAs(foo);
            theCache.GetObject(typeof (IFoo), instance).ShouldBeTheSameAs(foo);
            theCache.GetObject(typeof (IFoo), instance).ShouldBeTheSameAs(foo);
            theCache.GetObject(typeof (IFoo), instance).ShouldBeTheSameAs(foo);
            theCache.GetObject(typeof (IFoo), instance).ShouldBeTheSameAs(foo);

            theResolver.VerifyAllExpectations();
        }

        [Test]
        public void get_default_if_it_does_not_already_exist()
        {
            var instance = new ConfiguredInstance(typeof (Foo));
            thePipeline.Stub(x => x.GetDefault(typeof (IFoo))).Return(instance);

            var foo = new Foo();

            theResolver.Stub(x => x.ResolveFromLifecycle(typeof (IFoo), instance)).Return(foo);

            theCache.GetDefault(typeof (IFoo), thePipeline)
                .ShouldBeTheSameAs(foo);
        }

        [Test]
        public void get_default_is_cached()
        {
            var instance = new ConfiguredInstance(typeof (Foo));
            thePipeline.Stub(x => x.GetDefault(typeof (IFoo))).Return(instance);

            var foo = new Foo();

            theResolver.Expect(x => x.ResolveFromLifecycle(typeof (IFoo), instance)).Return(foo)
                .Repeat.Once();

            theCache.GetDefault(typeof (IFoo), thePipeline).ShouldBeTheSameAs(foo);
            theCache.GetDefault(typeof (IFoo), thePipeline).ShouldBeTheSameAs(foo);
            theCache.GetDefault(typeof (IFoo), thePipeline).ShouldBeTheSameAs(foo);
            theCache.GetDefault(typeof (IFoo), thePipeline).ShouldBeTheSameAs(foo);


            theResolver.VerifyAllExpectations();
        }

        [Test]
        public void start_with_explicit_args()
        {
            var foo1 = new Foo();

            var args = new ExplicitArguments();
            args.Set<IFoo>(foo1);

            theCache = new SessionCache(theResolver, args);

            thePipeline.Stub(x => x.GetDefault(typeof (IFoo))).Throw(new NotImplementedException());

            theCache.GetDefault(typeof (IFoo), thePipeline)
                .ShouldBeTheSameAs(foo1);
        }

        [Test]
        public void try_get_default_completely_negative_case()
        {
            theCache.TryGetDefault(typeof (IFoo), thePipeline).ShouldBeNull();
        }

        [Test]
        public void try_get_default_with_explicit_arg()
        {
            var foo1 = new Foo();

            var args = new ExplicitArguments();
            args.Set<IFoo>(foo1);

            theCache = new SessionCache(theResolver, args);

            theCache.GetDefault(typeof (IFoo), thePipeline)
                .ShouldBeTheSameAs(foo1);
        }

        [Test]
        public void try_get_default_with_a_default()
        {
            var instance = new ConfiguredInstance(typeof (Foo));
            thePipeline.Stub(x => x.GetDefault(typeof (IFoo))).Return(instance);

            var foo = new Foo();

            theResolver.Expect(x => x.ResolveFromLifecycle(typeof (IFoo), instance)).Return(foo)
                .Repeat.Once();

            theCache.TryGetDefault(typeof (IFoo), thePipeline)
                .ShouldBeTheSameAs(foo);
        }

        [Test]
        public void explicit_wins_over_instance_in_try_get_default()
        {
            var foo1 = new Foo();

            var args = new ExplicitArguments();
            args.Set<IFoo>(foo1);

            theCache = new SessionCache(theResolver, args);

            var instance = new ConfiguredInstance(typeof (Foo));
            thePipeline.Stub(x => x.GetDefault(typeof (IFoo))).Return(instance);

            var foo2 = new Foo();

            theResolver.Expect(x => x.ResolveFromLifecycle(typeof (IFoo), instance)).Return(foo2)
                .Repeat.Once();

            theCache.GetDefault(typeof (IFoo), thePipeline)
                .ShouldBeTheSameAs(foo1);
        }

        [Test]
        public void should_throw_configuration_exception_if_you_try_to_build_the_default_of_something_that_does_not_exist()
        {
            var ex = Exception<StructureMapConfigurationException>.ShouldBeThrownBy(() => theCache.GetDefault(typeof (IFoo), thePipeline));

            ex.Title.ShouldEqual("No default Instance is registered and cannot be automatically determined for type 'StructureMap.Testing.SessionCacheTester+IFoo'");
        }


        public interface IFoo
        {
        }

        public class Foo : IFoo
        {
        }
    }
}