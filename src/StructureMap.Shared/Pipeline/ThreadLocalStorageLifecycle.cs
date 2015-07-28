using System.Threading;

namespace StructureMap.Pipeline
{
    public class ThreadLocalStorageLifecycle : LifecycleBase
    {
        private static readonly ThreadLocal<IObjectCache> Cache = new ThreadLocal<IObjectCache>(() => new LifecycleObjectCache());

        public override void EjectAll(ILifecycleContext context)
        {
            FindCache(context).DisposeAndClear();
        }

        public override IObjectCache FindCache(ILifecycleContext context)
        {
            return Cache.Value;
        }

        public static void RefreshCache()
        {
            Cache.Value.DisposeAndClear();
        }
    }
}