using System;
using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    // In this case, per request/transient scoped objects need to be created within the scope of the
    // current session
    public class ContainerSpecificObjectCache : LifecycleObjectCache, ITransientTracking
    {
        protected override object buildWithSession(Type pluginType, Instance instance, IBuildSession session)
        {
            return session.BuildNewInSession(pluginType, instance);
        }

        public void Release(object o)
        {
            // nothing
        }

        public IEnumerable<object> Tracked
        {
            get { return new Object[0]; }
        }
    }
}