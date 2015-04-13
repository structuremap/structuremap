namespace StructureMap.Pipeline
{
    public abstract class LifecycleBase : ILifecycle
    {
        public string Description
        {
            get { return GetType().Name.Replace("Lifecycle", string.Empty); }
        }

        public abstract void EjectAll(ILifecycleContext context);
        public abstract IObjectCache FindCache(ILifecycleContext context);
    }
}