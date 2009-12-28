using System;

namespace StructureMap.Pipeline
{
    /// <summary>
    /// Makes sure that every request for this object returns a unique object
    /// </summary>
    public class UniquePerRequestLifecycle : ILifecycle
    {
        public void EjectAll()
        {
        }

        public IObjectCache FindCache()
        {
            return new NulloObjectCache();
        }

        public string Scope
        {
            get { return InstanceScope.Unique.ToString(); } }


    }
}