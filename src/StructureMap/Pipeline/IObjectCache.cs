using System;

namespace StructureMap.Pipeline
{
    public interface IObjectCache
    {
        object Locker { get; }

        int Count { get; }

        bool Has(Type pluginType, Instance instance);

        void Eject(Type pluginType, Instance instance);


        object Get(Type pluginType, Instance instance);
        void Set(Type pluginType, Instance instance, object value);
        void DisposeAndClear();
    }
}