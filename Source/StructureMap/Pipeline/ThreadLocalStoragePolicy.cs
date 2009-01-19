using System;

namespace StructureMap.Pipeline
{
    public class ThreadLocalStoragePolicy : CacheInterceptor
    {
        [ThreadStatic] private static InstanceCache _cache;
        private readonly object _locker = new object();

        public static void DisposeAndClearAll()
        {
            _cache.DisposeAndClear();
        }

        private void guaranteeHashExists()
        {
            if (_cache == null)
            {
                lock (_locker)
                {
                    if (_cache == null)
                    {
                        _cache = buildNewCache();
                    }
                }
            }
        }

        protected override InstanceCache findCache()
        {
            guaranteeHashExists();
            return _cache;
        }

        protected override CacheInterceptor clone()
        {
            return this;
        }
    }
}