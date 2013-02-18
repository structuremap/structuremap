using System;
using NUnit.Framework;
using Rhino.Mocks;
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
            var instance = new ConfiguredInstance(typeof(Foo));

            var foo = new Foo();

            theResolver.Stub(x => x.Resolve(typeof(IFoo), instance)).Return(foo);


            theCache.GetObject(typeof (IFoo), instance).ShouldBeTheSameAs(foo);
        }

        [Test]
        public void get_instance_remembers_the_first_object_created()
        {
            var instance = new ConfiguredInstance(typeof(Foo));

            var foo = new Foo();

            theResolver.Expect(x => x.Resolve(typeof(IFoo), instance)).Return(foo).Repeat.Once();


            theCache.GetObject(typeof(IFoo), instance).ShouldBeTheSameAs(foo);
            theCache.GetObject(typeof(IFoo), instance).ShouldBeTheSameAs(foo);
            theCache.GetObject(typeof(IFoo), instance).ShouldBeTheSameAs(foo);
            theCache.GetObject(typeof(IFoo), instance).ShouldBeTheSameAs(foo);
            theCache.GetObject(typeof(IFoo), instance).ShouldBeTheSameAs(foo);

            theResolver.VerifyAllExpectations();
        }

        [Test]
        public void get_default_if_it_does_not_already_exist()
        {
            var instance = new ConfiguredInstance(typeof (Foo));
            thePipeline.Stub(x => x.GetDefault(typeof (IFoo))).Return(instance);

            var foo = new Foo();

            theResolver.Stub(x => x.Resolve(typeof (IFoo), instance)).Return(foo);

            theCache.GetDefault(typeof (IFoo), thePipeline)
                    .ShouldBeTheSameAs(foo);
        }

        [Test]
        public void get_default_is_cached()
        {
            var instance = new ConfiguredInstance(typeof(Foo));
            thePipeline.Stub(x => x.GetDefault(typeof(IFoo))).Return(instance);

            var foo = new Foo();

            theResolver.Expect(x => x.Resolve(typeof(IFoo), instance)).Return(foo)
                .Repeat.Once();

            theCache.GetDefault(typeof(IFoo), thePipeline).ShouldBeTheSameAs(foo);
            theCache.GetDefault(typeof(IFoo), thePipeline).ShouldBeTheSameAs(foo);
            theCache.GetDefault(typeof(IFoo), thePipeline).ShouldBeTheSameAs(foo);
            theCache.GetDefault(typeof(IFoo), thePipeline).ShouldBeTheSameAs(foo);


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

        public interface IFoo{}
        public class Foo : IFoo{}
    }
}