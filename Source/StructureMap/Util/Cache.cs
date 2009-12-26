using System;
using System.Collections.Generic;
using System.Threading;

namespace StructureMap.Util
{
    [Serializable]
    public class Cache<TKey, TValue> where TValue : class
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly IDictionary<TKey, TValue> _values;
        private Func<TValue, TKey> _getKey = delegate { throw new NotImplementedException(); };

        private Action<TValue> _onAddition = x => { };

        private Func<TKey, TValue> _onMissing = delegate(TKey key)
        {
            string message = string.Format("Key '{0}' could not be found", key);
            throw new KeyNotFoundException(message);
        };

        public Cache()
            : this(new Dictionary<TKey, TValue>())
        {
        }

        public Cache(Func<TKey, TValue> onMissing)
            : this(new Dictionary<TKey, TValue>(), onMissing)
        {
        }

        public Cache(IDictionary<TKey, TValue> dictionary, Func<TKey, TValue> onMissing)
            : this(dictionary)
        {
            _onMissing = onMissing;
        }

        public Cache(IDictionary<TKey, TValue> dictionary)
        {
            _values = dictionary;
        }

        public Func<TKey, TValue> OnMissing { set { _onMissing = value; } }

        public Action<TValue> OnAddition { set { _onAddition = value; } }

        public Func<TValue, TKey> GetKey { get { return _getKey; } set { _getKey = value; } }

        public int Count
        {
            get
            {
                using (ReadLock())
                    return _values.Count;
            }
        }

        public TValue First
        {
            get
            {
                using (ReadLock())
                    foreach (var pair in _values)
                    {
                        return pair.Value;
                    }

                return null;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                using (ReadLockToken @lock = ReadLock())
                {
                    if (!_values.ContainsKey(key))
                    {
                        @lock.Upgrade();
                        if (!_values.ContainsKey(key))
                        {
                            TValue value = _onMissing(key);
                            _values.Add(key, value);
                        }
                    }

                    return _values[key];
                }
            }
            set
            {
                _onAddition(value);

                using (WriteLockToken @lock = WriteLock())
                {
                    if (_values.ContainsKey(key))
                    {
                        _values[key] = value;
                    }
                    else
                    {
                        _values.Add(key, value);
                    }
                }
            }
        }

        public void Fill(TKey key, TValue value)
        {
            using (WriteLock())
            {
                if (_values.ContainsKey(key))
                {
                    return;
                }

                _values.Add(key, value);
            }
        }

        public void Each(Action<TValue> action)
        {
            using (ReadLock())
            {
                foreach (var pair in _values)
                {
                    action(pair.Value);
                }
            }
        }

        public void Each(Action<TKey, TValue> action)
        {
            using (ReadLock())
            {
                foreach (var pair in _values)
                {
                    action(pair.Key, pair.Value);
                }
            }
        }

        public bool Has(TKey key)
        {
            using (ReadLock())
                return _values.ContainsKey(key);
        }

        public bool Exists(Predicate<TValue> predicate)
        {
            bool returnValue = false;

            Each(delegate(TValue value) { returnValue |= predicate(value); });

            return returnValue;
        }

        public TValue Find(Predicate<TValue> predicate)
        {
            using (ReadLock())
                foreach (var pair in _values)
                {
                    if (predicate(pair.Value))
                    {
                        return pair.Value;
                    }
                }

            return null;
        }

        public TKey Find(TValue value)
        {
            using (ReadLock())
                foreach (var pair in _values)
                {
                    if (pair.Value == value)
                    {
                        return pair.Key;
                    }
                }

            return default(TKey);
        }

        public TValue[] GetAll()
        {
            var returnValue = new TValue[Count];
            using (ReadLock())
                _values.Values.CopyTo(returnValue, 0);

            return returnValue;
        }

        public void Remove(TKey key)
        {
            using (ReadLock())
                if (_values.ContainsKey(key))
                {
                    _values.Remove(key);
                }
        }

        public void Clear()
        {
            using (WriteLock())
                _values.Clear();
        }

        public void WithValue(TKey key, Action<TValue> callback)
        {
            using (ReadLock())
                _values.TryGet(key, callback);
        }

        private ReadLockToken ReadLock()
        {
            return new ReadLockToken(_lock);
        }

        private WriteLockToken WriteLock()
        {
            return new WriteLockToken(_lock);
        }

        public Cache<TKey, TValue> Clone()
        {
            var cache = new Cache<TKey, TValue>();
            cache._onMissing = _onMissing;

            Each((key, value) => cache[key] = value);

            return cache;
        }

        #region Nested type: ReadLockToken

        private class ReadLockToken : IDisposable
        {
            private readonly ReaderWriterLockSlim _lock;
            private bool upgraded;

            public ReadLockToken(ReaderWriterLockSlim @lock)
            {
                _lock = @lock;
                _lock.EnterReadLock();
            }

            public void Dispose()
            {
                if (upgraded)
                    _lock.ExitWriteLock();
                else
                    _lock.ExitReadLock();
            }

            public void Upgrade()
            {
                _lock.ExitReadLock();
                _lock.EnterWriteLock();
                upgraded = true;
            }
        }

        #endregion

        #region Nested type: WriteLockToken

        private class WriteLockToken : IDisposable
        {
            private readonly ReaderWriterLockSlim _lock;

            public WriteLockToken(ReaderWriterLockSlim @lock)
            {
                _lock = @lock;
                _lock.EnterWriteLock();
            }

            public void Dispose()
            {
                _lock.EnterWriteLock();
            }
        }

        #endregion
    }
}