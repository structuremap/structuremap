namespace StructureMap.Pipeline
{
    /// <summary>
    /// "Singleton" for a specific child container or profile container
    /// </summary>
    public class ContainerLifecycle : LifecycleBase
    {
        public override void EjectAll(ILifecycleContext context)
        {
            FindCache(context).DisposeAndClear();
        }

        public override IObjectCache FindCache(ILifecycleContext context)
        {
            return context.ContainerCache;
        }
    }
}