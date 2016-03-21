using Rhino.Mocks;
using StructureMap.Pipeline;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class SingletonLifecycleTester
    {
        private readonly ILifecycleContext theContext;
        private readonly SingletonLifecycle theLifecycle;
        private readonly IObjectCache theCache;

        public SingletonLifecycleTester()
        {
            theContext = MockRepository.GenerateMock<ILifecycleContext>();
            theLifecycle = new SingletonLifecycle();

            theCache = MockRepository.GenerateMock<IObjectCache>();
            theContext.Stub(x => x.Singletons).Return(theCache);
        }

        [Fact]
        public void the_cache_is_from_the_transient_of_the_context()
        {
            theLifecycle.FindCache(theContext).ShouldBeTheSameAs(theCache);
        }

        [Fact]
        public void eject_all_delegates()
        {
            theLifecycle.EjectAll(theContext);

            theCache.AssertWasCalled(x => x.DisposeAndClear());
        }
    }
}