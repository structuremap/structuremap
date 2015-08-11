using System;
using StructureMap.Pipeline;

namespace StructureMap
{
    public interface IBuildSession
    {
        object BuildNewInSession(Type pluginType, Instance instance);
        object BuildNewInOriginalContext(Type pluginType, Instance instance);
        object ResolveFromLifecycle(Type pluginType, Instance instance);
        Policies Policies { get; }
        object CreateInstance(Type pluginType, string name);

        void Push(Instance instance);
        void Pop();
    }
}