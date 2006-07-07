using System.Collections.Specialized;

namespace StructureMap.Caching
{
	public abstract class EventDispatcher
	{
		private HybridDictionary _caches;
		private string _subjectName;

		public EventDispatcher(string SubjectName)
		{
			_caches = new HybridDictionary();
			_subjectName = SubjectName;
		}

		public string SubjectName
		{
			get { return _subjectName; }
		}

		public void AddManagedCache(IManagedCache Cache)
		{
			_caches.Add(Cache.CacheName, Cache);
		}

		public void RemoveCache(string CacheName)
		{
			lock (_caches)
			{
				if (_caches.Contains(CacheName))
				{
					_caches.Remove(CacheName);
				}
			}
		}

		public void Dispatch()
		{
			lock (_caches.SyncRoot)
			{
				foreach (IManagedCache _cache in _caches.Values)
				{
					dispatch(_cache);
				}
			}
		}

		protected abstract void dispatch(IManagedCache _cache);

	}
}