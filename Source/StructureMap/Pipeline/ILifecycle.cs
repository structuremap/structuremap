using StructureMap.Attributes;

namespace StructureMap.Pipeline
{
    public interface ILifecycle
    {
        void EjectAll();
        IObjectCache FindCache();
        string Scope { get; }
    }
}