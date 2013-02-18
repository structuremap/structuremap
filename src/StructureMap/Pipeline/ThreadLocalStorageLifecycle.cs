using System;

namespace StructureMap.Pipeline
{
    public class ThreadLocalStorageLifecycle : ILifecycle
    {
        [ThreadStatic] private static LifecycleObjectCache _cache;
        private readonly object _locker = new object();

        public void EjectAll(ILifecycleContext context)
        {
            FindCache(context).DisposeAndClear();
        }

        public IObjectCache FindCache(ILifecycleContext context)
        {
            guaranteeHashExists();
            return _cache;
        }

        public string Scope { get { return InstanceScope.ThreadLocal.ToString(); } }

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