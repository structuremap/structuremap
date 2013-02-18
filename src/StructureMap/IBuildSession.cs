using System;
using StructureMap.Pipeline;

namespace StructureMap
{
    public interface IBuildSession
    {
        object Build(Type pluginType, Instance instance);
        object BuildInOriginalContext(Type pluginType, Instance instance);
        object Resolve(Type pluginType, Instance instance);
    }
}