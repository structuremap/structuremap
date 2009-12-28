namespace StructureMap.Pipeline
{
    public interface ILifecycle
    {
        string Scope { get; }
        void EjectAll();
        IObjectCache FindCache();
    }
}