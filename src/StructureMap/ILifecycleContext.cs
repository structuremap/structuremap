using System;
using StructureMap.Pipeline;

namespace StructureMap
{
    public interface ILifecycleContext
    {
        IObjectCache Singletons { get; }
        IObjectCache Transients { get; }

        ILifecycle DetermineLifecycle(Type pluginType, Instance instance);
    }
}