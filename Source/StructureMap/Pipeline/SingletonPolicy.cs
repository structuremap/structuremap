using System;
using StructureMap.Attributes;

namespace StructureMap.Pipeline
{
    [Obsolete("Kill!")]
    public class SingletonPolicy : CacheInterceptor
    {
        private readonly object _locker = new object();
        private ObjectCache _cache;

        protected override ObjectCache findCache()
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

            return _cache;
        }

        public ObjectCache Cache
        {
            get
            {
                return findCache();
            }
        }

        protected override void ejectAll()
        {
            lock (_locker)
            {
                _cache.DisposeAndClear();
            }
        }

        protected override CacheInterceptor clone()
        {
            return new SingletonPolicy();
        }

        public override string ToString()
        {
            return InstanceScope.Singleton.ToString();
        }
    }
}