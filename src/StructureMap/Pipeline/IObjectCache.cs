using System;

namespace StructureMap.Pipeline
{
    public interface IObjectCache
    {
        [Obsolete("this is an abomination")]
        object Locker { get; }
        object Get(Type pluginType, Instance instance);
        void Set(Type pluginType, Instance instance, object value);

        int Count { get; }

        bool Has(Type pluginType, Instance instance);

        void Eject(Type pluginType, Instance instance);


        
        void DisposeAndClear();
    }
}