namespace StructureMap.Caching
{
    public interface IStorageStrategy
    {
        ICacheItem BuildCacheItem(object Key);
    }
}