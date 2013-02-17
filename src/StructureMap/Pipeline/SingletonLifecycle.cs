namespace StructureMap.Pipeline
{
    public class SingletonLifecycle : ILifecycle
    {
        private readonly MainObjectCache _cache = new MainObjectCache();

        public void EjectAll(ILifecycleContext context)
        {
            _cache.DisposeAndClear();
        }

        public IObjectCache FindCache(ILifecycleContext context)
        {
            return _cache;
        }

        public string Scope { get { return InstanceScope.Singleton.ToString(); } }
    }
}