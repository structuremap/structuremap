using Rhino.Mocks;
using StructureMap.Pipeline;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class TransientLifecycleTester
    {
        private ILifecycleContext theContext;
        private TransientLifecycle theLifecycle;
        private ITransientTracking theCache;

        public TransientLifecycleTester()
        {
            theContext = MockRepository.GenerateMock<ILifecycleContext>();
            theLifecycle = new TransientLifecycle();

            theCache = MockRepository.GenerateMock<ITransientTracking>();
            theContext.Stub(x => x.Transients).Return(theCache);
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