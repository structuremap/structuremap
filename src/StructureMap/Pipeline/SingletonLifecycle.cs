namespace StructureMap.Pipeline
{
    public class SingletonLifecycle : LifecycleBase
    {
        public override void EjectAll(ILifecycleContext context)
        {
            FindCache(context).DisposeAndClear();
        }

        public override IObjectCache FindCache(ILifecycleContext context)
        {
            return context.Singletons;
        }
    }
}