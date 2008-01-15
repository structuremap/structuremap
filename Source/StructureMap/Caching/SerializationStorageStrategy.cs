namespace StructureMap.Caching
{
    public class SerializationStorageStrategy : IStorageStrategy
    {
        #region IStorageStrategy Members

        public ICacheItem BuildCacheItem(object Key)
        {
            return new SerializationCacheItem(Key);
        }

        #endregion
    }
}