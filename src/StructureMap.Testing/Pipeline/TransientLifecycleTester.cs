#if NET451
using NSubstitute;
using StructureMap.Pipeline;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class TransientLifecycleTester
    {
        private readonly ILifecycleContext theContext;
        private readonly TransientLifecycle theLifecycle;
        private readonly ITransientTracking theCache;

        public TransientLifecycleTester()
        {
            theContext = Substitute.For<ILifecycleContext>();
            theLifecycle = new TransientLifecycle();

            theCache = Substitute.For<ITransientTracking>();
            theContext.Transients.Returns(theCache);
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

            theCache.Received().DisposeAndClear();
        }
    }
}
#endif