using System;

namespace StructureMap
{
    public interface IGraphEjector
    {
        void EjectAllInstancesOf<T>();

        void Remove(Func<Type, bool> filter);
        void Remove(Type pluginType);
    }
}