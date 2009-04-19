using System;
using StructureMap.Attributes;

namespace StructureMap.Pipeline
{
    public class ThreadLocalStoragePolicy : CacheInterceptor
    {
        [ThreadStatic] private static ObjectCache _cache;
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

        protected override ObjectCache findCache()
        {
            guaranteeHashExists();
            return _cache;
        }

        protected override CacheInterceptor clone()
        {
            return this;
        }

        public override string ToString()
        {
            return InstanceScope.Hybrid.ToString();
        }
    }
}