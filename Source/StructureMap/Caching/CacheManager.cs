using System.Collections.Specialized;
using System.Threading;

namespace StructureMap.Caching
{
	public class CacheManager
	{
		#region static

		// TODO -- How to get the polling interval time?
		private static CacheManager _instance = new CacheManager(5);

		public static CacheManager CurrentManager
		{
			get { return _instance; }
		}

		#endregion

		private HybridDictionary _dispatchers;
		private bool _continuePolling = true;
		private bool _isPolling = false;
		private int _PollingTimeInMinutes;
		private EventDispatcher _clearAllDispatcher;
		private EventDispatcher _pruneAllDispatcher;


		public CacheManager(int PollingTimeInMinutes)
		{
			_dispatchers = new HybridDictionary();
			_PollingTimeInMinutes = PollingTimeInMinutes;

			_clearAllDispatcher = new ClearEventDispatcher("CLEAR_ALL");
			addDispatcher(_clearAllDispatcher);

			_pruneAllDispatcher = new PruneEventDispatcher("PRUNE_ALL");
			addDispatcher(_pruneAllDispatcher);
		}

		private void addDispatcher(EventDispatcher dispatcher)
		{
			_dispatchers.Add(dispatcher.SubjectName, dispatcher);
		}


		public void ManageCache(IManagedCache Cache)
		{
			this.ReleaseCache(Cache.CacheName);

			_clearAllDispatcher.AddManagedCache(Cache);
			_pruneAllDispatcher.AddManagedCache(Cache);

			Cache.AddWatches(this);
		}

		public void WatchFile(string FilePath, IManagedCache Cache)
		{
			string subject = FileModificationWatcher.SubjectNameFromFilePath(FilePath);

			EventDispatcher dispatcher = null;
			if (!_dispatchers.Contains(subject))
			{
				lock (this)
				{
					if (!_dispatchers.Contains(subject))
					{
						dispatcher = new FileModificationWatcher(FilePath);
						addDispatcher(dispatcher);
					}
				}
			}

			if (dispatcher == null)
			{
				dispatcher = (EventDispatcher) _dispatchers[subject];
			}

			dispatcher.AddManagedCache(Cache);
		}

		public void ReleaseCache(string CacheName)
		{
			foreach (EventDispatcher _dispatcher in _dispatchers.Values)
			{
				_dispatcher.RemoveCache(CacheName);
			}
		}

		public void ClearAll()
		{
			_clearAllDispatcher.Dispatch();
		}

		#region polling

		public void StartPolling()
		{
			if (!this.IsPolling)
			{
				_continuePolling = true;
				Thread newThread = new Thread(new ThreadStart(this.poll));
				newThread.Start();
			}
		}

		public void StopPolling()
		{
			_continuePolling = false;
		}

		public bool IsPolling
		{
			get { return _isPolling; }
		}

		public int PollingTimeInMinutes
		{
			get { return _PollingTimeInMinutes; }
		}


		private void poll()
		{
			while (_continuePolling)
			{
				Thread.Sleep(_PollingTimeInMinutes*60000);

				if (_continuePolling)
				{
					_pruneAllDispatcher.Dispatch();
				}
			}

			_isPolling = false;
		}

		#endregion

		public void DispatchEvent(string EventName)
		{
			if (_dispatchers.Contains(EventName))
			{
				EventDispatcher _dispatcher = (EventDispatcher) _dispatchers[EventName];
				_dispatcher.Dispatch();
			}
		}
	}


}