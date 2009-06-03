using System;

namespace StructureMap.Pipeline
{
    public class NulloObjectCache : IObjectCache
    {
        public object Locker
        {
            get { return new object(); }
        }

        public int Count
        {
            get { return 0; }
        }

        public object Get(Type pluginType, Instance instance)
        {
            return null;
        }

        public void Set(Type pluginType, Instance instance, object value)
        {
            // no-op
        }

        public void DisposeAndClear()
        {
            // no-op
        }
    }
}