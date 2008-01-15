namespace StructureMap.Caching
{
    public class SharedStorageStrategy : IStorageStrategy
    {
        #region IStorageStrategy Members

        public ICacheItem BuildCacheItem(object Key)
        {
            return new SharedCacheItem(Key);
        }

        #endregion
    }
}