using System;
using StructureMap.Caching;

namespace StructureMap
{
    internal class ObjectFactoryCacheCallback : IManagedCache
    {
        public ObjectFactoryCacheCallback()
        {
            try
            {
                CacheManager.CurrentManager.WatchFile(StructureMapConfiguration.GetStructureMapConfigurationPath(), this);
            }
            catch (Exception exception)
            {
                Console.Write(exception);
            }
        }

        #region IManagedCache Members

        public string CacheName
        {
            get { return "ObjectFactory"; }
        }

        public void Clear()
        {
            ObjectFactory.Reset();
        }

        //no-op for interface implementation
        public void Prune(DateTime currentTime)
        {
        }

        public void AddWatches(CacheManager Manager)
        {
        }

        #endregion
    }
}