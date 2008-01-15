namespace StructureMap.Caching
{
    public class CloneStorageStrategy : IStorageStrategy
    {
        #region IStorageStrategy Members

        public ICacheItem BuildCacheItem(object Key)
        {
            return new CloneCacheItem(Key);
        }

        #endregion
    }
}