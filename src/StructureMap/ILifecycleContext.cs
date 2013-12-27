using StructureMap.Pipeline;

namespace StructureMap
{
    public interface ILifecycleContext
    {
        IObjectCache Singletons { get; }
        IObjectCache Transients { get; }
    }
}