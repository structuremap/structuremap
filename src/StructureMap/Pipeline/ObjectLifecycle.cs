using System;

namespace StructureMap.Pipeline
{
    /// <summary>
    /// Used internally to mark objects that are injected directly into the container
    /// </summary>
    public class ObjectLifecycle : LifecycleBase
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