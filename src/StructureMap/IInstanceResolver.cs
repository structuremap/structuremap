using System;
using StructureMap.Pipeline;

namespace StructureMap
{
    public interface IInstanceResolver
    {
        // TODO -- add a new ResolveInIsolatedContext
        object Resolve(Type pluginType, Instance instance);
    }
}