using System;

namespace StructureMap.Pipeline
{
    /// <summary>
    /// Makes sure that every request for this object returns a unique object
    /// </summary>
    public class UniquePerRequestLifecycle : LifecycleBase, IAppropriateForNestedContainer
    {
        public override void EjectAll(ILifecycleContext context)
        {
        }

        public override IObjectCache FindCache(ILifecycleContext context)
        {
            return new NulloObjectCache();
        }

        public class NulloObjectCache : IObjectCache
        {
            public object Get(Type pluginType, Instance instance, IBuildSession session)
            {
                return session.BuildNewInSession(pluginType, instance);
            }

            public int Count { get; } = 0;
            public bool Has(Type pluginType, Instance instance)
            {
                return false;
            }

            public void Eject(Type pluginType, Instance instance)
            {
                // nothing
            }

            public void DisposeAndClear()
            {
                // nothing
            }
        }
    }
}