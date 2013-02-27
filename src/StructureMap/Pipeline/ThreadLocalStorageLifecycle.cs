using System;

namespace StructureMap.Pipeline
{
    public class ThreadLocalStorageLifecycle : LifecycleBase
    {
        [ThreadStatic] private static LifecycleObjectCache _cache;
        private readonly object _locker = new object();

        public override void EjectAll(ILifecycleContext context)
        {
            FindCache(context).DisposeAndClear();
        }

        public override IObjectCache FindCache(ILifecycleContext context)
        {
            guaranteeHashExists();
            return _cache;
        }

        private void guaranteeHashExists()
        {
            if (_cache == null)
            {
                lock (_locker)
                {
                    if (_cache == null)
                    {
                        _cache = new LifecycleObjectCache();
                    }
                }
            }
        }
    }
}