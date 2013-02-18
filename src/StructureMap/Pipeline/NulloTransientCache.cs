using System;

namespace StructureMap.Pipeline
{
    public class NulloTransientCache : IObjectCache
    {
        public object Locker { get { return new object(); } }

        public int Count { get { return 0; } }

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
    }
}