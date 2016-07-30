using System;
using StructureMap.Pipeline;

namespace StructureMap
{
    public interface IGraphEjector
    {
        void EjectAllInstancesOf<T>();

        void RemoveCompletely(Func<Type, bool> filter);
        void RemoveCompletely(Type pluginType);
        void RemoveFromLifecycle(Type pluginType, Instance instance);
        void RemoveCompletely(Type pluginType, Instance instance);
    }
}