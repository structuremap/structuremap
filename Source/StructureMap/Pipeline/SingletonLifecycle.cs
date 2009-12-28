namespace StructureMap.Pipeline
{
    public class SingletonLifecycle : ILifecycle
    {
        private readonly MainObjectCache _cache = new MainObjectCache();

        public void EjectAll()
        {
            _cache.DisposeAndClear();
        }

        public IObjectCache FindCache()
        {
            return _cache;
        }

        public string Scope { get { return InstanceScope.Singleton.ToString(); } }
    }
}