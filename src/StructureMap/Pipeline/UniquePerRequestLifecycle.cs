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
            throw new NotSupportedException("Should never be called");
        }
    }
}