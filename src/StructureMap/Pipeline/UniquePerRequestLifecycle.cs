using System;

namespace StructureMap.Pipeline
{
    /// <summary>
    /// Makes sure that every request for this object returns a unique object
    /// </summary>
    public class UniquePerRequestLifecycle : ILifecycle
    {
        public void EjectAll(ILifecycleContext context)
        {
        }

        public IObjectCache FindCache(ILifecycleContext context)
        {
            throw new NotSupportedException("Should never be called");
        }

        public string Scope { get { return InstanceScope.Unique.ToString(); } }
    }
}