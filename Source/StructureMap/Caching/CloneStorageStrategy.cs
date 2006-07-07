namespace StructureMap.Caching
{
	public class CloneStorageStrategy : IStorageStrategy
	{
		public ICacheItem BuildCacheItem(object Key)
		{
			return new CloneCacheItem(Key);
		}
	}
}