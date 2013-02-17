namespace StructureMap.Pipeline
{
    public class SingletonLifecycle : ILifecycle
    {
        public void EjectAll(ILifecycleContext context)
        {
            FindCache(context).DisposeAndClear();
        }

        public IObjectCache FindCache(ILifecycleContext context)
        {
            return context.Singletons;
        }

        public string Scope { get { return InstanceScope.Singleton.ToString(); } }
    }
}