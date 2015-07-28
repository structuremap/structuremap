namespace StructureMap.Pipeline
{
    public class TransientLifecycle : LifecycleBase, IAppropriateForNestedContainer
    {
        public override void EjectAll(ILifecycleContext context)
        {
            FindCache(context).DisposeAndClear();
        }

        public override IObjectCache FindCache(ILifecycleContext context)
        {
            return context.Transients;
        }
    }
}