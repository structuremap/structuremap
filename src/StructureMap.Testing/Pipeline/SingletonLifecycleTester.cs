#if NET451
using NSubstitute;
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
            theContext = Substitute.For<ILifecycleContext>();
            theLifecycle = new SingletonLifecycle();

            theCache = Substitute.For<IObjectCache>();
            theContext.Singletons.Returns(theCache);
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