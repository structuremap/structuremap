using System;

namespace StructureMap.Caching
{
    public interface IManagedCache
    {
        string CacheName { get; }
        void Clear();
        void Prune(DateTime currentTime);
        void AddWatches(CacheManager Manager);
    }
}