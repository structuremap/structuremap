namespace StructureMap.Pipeline
{
    public class ChildContainerSingletonLifecycle : LifecycleBase
    {
        private readonly IObjectCache _cache;

        public ChildContainerSingletonLifecycle(IObjectCache cache)
        {
            _cache = cache;
        }

        public override void EjectAll(ILifecycleContext context)
        {
            _cache.DisposeAndClear();
        }

        public override IObjectCache FindCache(ILifecycleContext context)
        {
            return _cache;
        }
    }
}