using System;
using StructureMap.Pipeline;

namespace StructureMap
{
    public interface ILifecycleContext
    {
        IObjectCache Singletons { get; }
        ITransientTracking Transients { get; }
        IObjectCache ContainerCache { get; }

        ILifecycle DetermineLifecycle(Type pluginType, Instance instance);
    }
}