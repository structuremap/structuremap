namespace StructureMap.Pipeline
{
    public interface ILifecycle
    {
        string Description { get; }
        void EjectAll(ILifecycleContext context);
        IObjectCache FindCache(ILifecycleContext context);
    }
}