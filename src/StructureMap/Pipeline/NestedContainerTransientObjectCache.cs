using System;

namespace StructureMap.Pipeline
{
    // In this case, per request/transient scoped objects need to be created within the scope of the
    // current session
    public class NestedContainerTransientObjectCache : LifecycleObjectCache
    {
        protected override object buildWithSession(Type pluginType, Instance instance, IBuildSession session)
        {
            return session.BuildNewInSession(pluginType, instance);
        }
    }
}