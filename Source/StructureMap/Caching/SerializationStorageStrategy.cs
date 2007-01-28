namespace StructureMap.Caching
{
    public class SerializationStorageStrategy : IStorageStrategy
    {
        public ICacheItem BuildCacheItem(object Key)
        {
            return new SerializationCacheItem(Key);
        }
    }
}