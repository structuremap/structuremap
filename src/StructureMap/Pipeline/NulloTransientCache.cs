using System;
using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public class NulloTransientCache : ITransientTracking
    {
        public object Locker
        {
            get { return new object(); }
        }

        public int Count
        {
            get { return 0; }
        }

        public bool Has(Type pluginType, Instance instance)
        {
            return false;
        }

        public void Eject(Type pluginType, Instance instance)
        {
        }

        public object Get(Type pluginType, Instance instance, IBuildSession session)
        {
            return session.BuildNewInSession(pluginType, instance);
        }

        public void DisposeAndClear()
        {
            // no-op
        }

        public void Release(object o)
        {
            // no-op
        }

        public IEnumerable<object> Tracked
        {
            get { return new Object[0]; }
        }

    }
}