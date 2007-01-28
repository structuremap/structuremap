namespace StructureMap.Caching
{
    public class SharedStorageStrategy : IStorageStrategy
    {
        public ICacheItem BuildCacheItem(object Key)
        {
            return new SharedCacheItem(Key);
        }
    }
}