using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class SingletonLifecycleTester
    {
        private ILifecycleContext theContext;
        private SingletonLifecycle theLifecycle;
        private IObjectCache theCache;

        [SetUp]
        public void SetUp()
        {
            theContext = MockRepository.GenerateMock<ILifecycleContext>();
            theLifecycle = new SingletonLifecycle();

            theCache = MockRepository.GenerateMock<IObjectCache>();
            theContext.Stub(x => x.Singletons).Return(theCache);
        }

        [Test]
        public void the_cache_is_from_the_transient_of_the_context()
        {
            theLifecycle.FindCache(theContext).ShouldBeTheSameAs(theCache);
        }

        [Test]
        public void eject_all_delegates()
        {
            theLifecycle.EjectAll(theContext);

            theCache.AssertWasCalled(x => x.DisposeAndClear());
        }

    }
}