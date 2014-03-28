using System;

namespace StructureMap.Pipeline
{
    // SAMPLE: IObjectCache
    public interface IObjectCache
    {
        object Get(Type pluginType, Instance instance, IBuildSession session);
        int Count { get; }
        bool Has(Type pluginType, Instance instance);
        void Eject(Type pluginType, Instance instance);
        void DisposeAndClear();
    }
    // ENDSAMPLE
}