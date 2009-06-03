namespace StructureMap.Pipeline
{
    public interface ILifecycle
    {
        void EjectAll();
        IObjectCache FindCache();
    }
}