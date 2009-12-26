using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using StructureMap.Graph;

namespace StructureMap.Util
{
    [Serializable]
    public class Cache<TKey, TValue> where TValue : class
    {
        readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        readonly IDictionary<TKey, TValue> _values;
        Func<TValue, TKey> _getKey = delegate { throw new NotImplementedException(); };

        Action<TValue> _onAddition = x => { };

        Func<TKey, TValue> _onMissing = delegate(TKey key)
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
                using (var @lock = ReadLock())
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

                using (var @lock = WriteLock())
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
                foreach (KeyValuePair<TKey, TValue> pair in _values)
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

        ReadLockToken ReadLock()
        {
            return new ReadLockToken(_lock);
        }

        WriteLockToken WriteLock()
        {
            return new WriteLockToken(_lock);
        }

        class ReadLockToken : IDisposable
        {
            readonly ReaderWriterLockSlim _lock;
            bool upgraded;

            public ReadLockToken(ReaderWriterLockSlim @lock)
            {
                _lock = @lock;
                _lock.EnterReadLock();
            }

            public void Upgrade()
            {
                _lock.ExitReadLock();
                _lock.EnterWriteLock();
                upgraded = true;
            }

            public void Dispose()
            {
                if (upgraded)
                    _lock.ExitWriteLock();
                else
                    _lock.ExitReadLock();
            }
        }

        class WriteLockToken : IDisposable
        {
            readonly ReaderWriterLockSlim _lock;
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

        public Cache<TKey, TValue> Clone()
        {
            var cache = new Cache<TKey, TValue>();
            cache._onMissing = _onMissing;
            
            Each((key, value) => cache[key] = value);

            return cache;
        }
    }
}
