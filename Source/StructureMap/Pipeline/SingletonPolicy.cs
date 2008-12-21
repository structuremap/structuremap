using System;

namespace StructureMap.Pipeline
{
    [Pluggable("Singleton")]
    public class SingletonPolicy : CacheInterceptor
    {
        private readonly object _locker = new object();
        private InstanceCache _cache;

        protected override InstanceCache findCache()
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

        public InstanceCache Cache
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
                _cache.Each(o =>
                {
                    IDisposable disposable = o as IDisposable;
                    if (disposable != null)
                    {
                        try
                        {
                            disposable.Dispose();
                        }
                        catch (Exception){}
                    }
                });

                _cache.Clear();
            }
        }

        protected override CacheInterceptor clone()
        {
            return new SingletonPolicy();
        }
    }
}