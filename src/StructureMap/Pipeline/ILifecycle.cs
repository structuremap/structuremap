namespace StructureMap.Pipeline
{
    public interface ILifecycle
    {
        string Scope { get; }
        void EjectAll(ILifecycleContext context);
        IObjectCache FindCache(ILifecycleContext context);
    }
}