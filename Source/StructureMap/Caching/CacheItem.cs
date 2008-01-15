using System;
using System.Threading;

namespace StructureMap.Caching
{
    public abstract class CacheItem : ICacheItem
    {
        private int _accesses;
        private DateTime _created;
        private bool _isEmpty = false;
        private object _key;
        private DateTime _lastAccessed;
        private ReaderWriterLock rwl;

        public CacheItem(object Key)
        {
            rwl = new ReaderWriterLock();
            _key = Key;

            reset();
        }

        public bool IsEmpty
        {
            get { return _isEmpty; }
        }

        #region ICacheItem Members

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

        public object Value
        {
            get
            {
                rwl.AcquireReaderLock(1000);
                object returnValue = getValue();
                rwl.ReleaseReaderLock();

                return returnValue;
            }
            set
            {
                rwl.AcquireWriterLock(1000);
                setValue(value);
                reset();
                _isEmpty = false;
                rwl.ReleaseWriterLock();
            }
        }

        #endregion

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


        protected abstract object getValue();
        protected abstract void setValue(object Value);
    }
}