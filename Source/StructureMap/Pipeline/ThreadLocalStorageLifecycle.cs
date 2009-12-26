using System;

namespace StructureMap.Pipeline
{
    public class ThreadLocalStorageLifecycle : ILifecycle
    {
        [ThreadStatic] private static MainObjectCache _cache;
        private readonly object _locker = new object();

        public void EjectAll()
        {
            FindCache().DisposeAndClear();
        }

        public IObjectCache FindCache()
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
                        _cache = new MainObjectCache();
                    }
                }
            }
        }
    }
}