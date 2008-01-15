using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace StructureMap.Caching
{
    public class LazyCache : IManagedCache
    {
        private IExpirationPolicy[] _expirations;
        private HybridDictionary _items;
        private string _name;
        private bool _refillOnExpiration;
        private IValueSource _source;
        private IStorageStrategy _storage;


        public LazyCache(string CacheName, IExpirationPolicy[] Expirations, IValueSource Source,
                         IStorageStrategy Storage, bool RefillOnExpiration)
        {
            _name = CacheName;
            _expirations = Expirations;
            _source = Source;
            _storage = Storage;
            _refillOnExpiration = RefillOnExpiration;
            _items = new HybridDictionary();

            CacheManager.CurrentManager.ManageCache(this);
        }

        [IndexerName("Value")]
        public object this[object key]
        {
            get
            {
                if (!_items.Contains(key))
                {
                    lock (_items.SyncRoot)
                    {
                        if (!_items.Contains(key))
                        {
                            ICacheItem newItem = _storage.BuildCacheItem(key);
                            newItem.Value = _source.GetValue(key);
                            _items.Add(key, newItem);
                        }
                    }
                }

                ICacheItem item = (ICacheItem) _items[key];
                return item.Value;
            }
            set
            {
                ICacheItem item = null;

                if (!_items.Contains(key))
                {
                    lock (_items.SyncRoot)
                    {
                        if (!_items.Contains(key))
                        {
                            item = _storage.BuildCacheItem(key);
                            _items.Add(key, item);
                        }
                    }
                }

                if (item == null)
                {
                    item = (ICacheItem) _items[key];
                }

                item.Value = value;
            }
        }

        #region IManagedCache Members

        public string CacheName
        {
            get { return _name; }
        }

        public void Clear()
        {
            lock (this)
            {
                _items.Clear();
            }
        }

        public void Prune(DateTime currentTime)
        {
            lock (this)
            {
                foreach (IExpirationPolicy policy in _expirations)
                {
                    policy.Calculate(currentTime);

                    foreach (ICacheItem item in _items.Values)
                    {
                        if (policy.HasExpired(item))
                        {
                            expire(item);
                        }
                    }
                }
            }
        }

        public void AddWatches(CacheManager Manager)
        {
            // TODO:  Add LazyCache.AddWatches implementation
        }

        #endregion

        private void expire(ICacheItem item)
        {
            if (_refillOnExpiration)
            {
                item.Value = _source.GetValue(item.Key);
            }
            else
            {
                _items.Remove(item);
            }
        }

        // TODO -- optimize this.  Go to record level locking
    }
}