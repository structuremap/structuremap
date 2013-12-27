using System;
using StructureMap.Pipeline;

namespace StructureMap
{
    public interface IBuildSession
    {
        object BuildNewInSession(Type pluginType, Instance instance);
        object BuildNewInOriginalContext(Type pluginType, Instance instance);
        object ResolveFromLifecycle(Type pluginType, Instance instance);
        string RequestedName { get; set; }
    }
}