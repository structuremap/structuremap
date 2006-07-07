using System;
using System.Threading;

namespace StructureMap.Caching
{
	public abstract class CacheItem : ICacheItem
	{
		private object _key;
		private DateTime _lastAccessed;
		private DateTime _created;
		private int _accesses;
		private ReaderWriterLock rwl;
		private bool _isEmpty = false;

		public CacheItem(object Key)
		{
			rwl = new ReaderWriterLock();
			_key = Key;

			reset();
		}

		public object Key
		{
			get { return _key; }
		}

		public DateTime LastAccessed
		{
			get { return _lastAccessed; }
		}

		public DateTime Created
		{
			get { return _created; }
		}

		public int Accesses
		{
			get { return _accesses; }
		}


		private void reset()
		{
			_accesses = 0;
			_lastAccessed = _created = DateTime.Now;
		}

		private void markAccess()
		{
			lock (this)
			{
				_accesses++;
				_lastAccessed = DateTime.Now;
			}
		}


		public object Value
		{
			get
			{
				rwl.AcquireReaderLock(1000);
				object returnValue = this.getValue();
				rwl.ReleaseReaderLock();

				return returnValue;
			}
			set
			{
				rwl.AcquireWriterLock(1000);
				this.setValue(value);
				this.reset();
				_isEmpty = false;
				rwl.ReleaseWriterLock();
			}
		}

		public bool IsEmpty
		{
			get { return _isEmpty; }
		}


		protected abstract object getValue();
		protected abstract void setValue(object Value);
	}
}