using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using StructureMap.Pipeline;

namespace StructureMap.Util
{
    public class Cache<TKey, TValue> : IEnumerable<TValue>
    {
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly IDictionary<TKey, TValue> _values;

        private Func<TValue, TKey> _getKey = delegate { throw new NotImplementedException(); };

        private Action<TValue> _onAddition = x => { };

        private Func<TKey, TValue> _onMissing = delegate(TKey key) {
            var message = string.Format("Key '{0}' could not be found", key);
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

        public Action<TValue> OnAddition
        {
            set { _onAddition = value; }
        }

        public Func<TKey, TValue> OnMissing
        {
            set { _onMissing = value; }
        }

        public Func<TValue, TKey> GetKey
        {
            get { return _getKey; }
            set { _getKey = value; }
        }

        public int Count
        {
            get
            {
                return _rwLock.Read(() => _values.Count);
            }
        }

        public TValue First
        {
            get
            {
                return _rwLock.Read(() => _values.Values.FirstOrDefault());
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                FillDefault(key);

                return _values[key];
            }
            set
            {
                _onAddition(value);
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TValue>) this).GetEnumerator();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _values.Values.GetEnumerator();
        }

        /// <summary>
        ///   Guarantees that the Cache has the default value for a given key.
        ///   If it does not already exist, it's created.
        /// </summary>
        /// <param name = "key"></param>
        public void FillDefault(TKey key)
        {
            Fill(key, _onMissing);
        }

        public void Fill(TKey key, Func<TKey, TValue> onMissing)
        {
            _rwLock.EnterUpgradeableReadLock();
            try
            {
                if (!_values.ContainsKey(key))
                {
                    var value = onMissing(key);
                    _onAddition(value);
                    Fill(key, value);
                }
            }
            finally
            {
                _rwLock.ExitUpgradeableReadLock();
            }
        }

        public void Fill(TKey key, TValue value)
        {
            if (_values.ContainsKey(key))
            {
                return;
            }

            _rwLock.EnterWriteLock();
            try
            {
                _values.Add(key, value);
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public void Each(Action<TValue> action)
        {
            var snapshot = _rwLock.Read(() => _values.Values.ToArray());
            snapshot.Each(action);
        }

        public void Each(Action<TKey, TValue> action)
        {
            var snapshot = _rwLock.Read(() => _values.ToArray());
            foreach (var pair in snapshot)
            {
                action(pair.Key, pair.Value);
            }
        }

        public bool Has(TKey key)
        {
            return _rwLock.Read(()=>_values.ContainsKey(key));
        }

        public bool Exists(Func<TValue,bool> predicate)
        {
            var snapshot = _rwLock.Read(() => _values.Values.ToArray());
            return snapshot.Any(predicate);
        }

        public TValue Find(Func<TValue,bool> predicate)
        {
            var snapshot = _rwLock.Read(() => _values.Values.ToArray());
            return snapshot.FirstOrDefault(predicate);
        }

        public TKey[] GetAllKeys()
        {
            return _rwLock.Read(() => _values.Keys.ToArray());
        }

        public TValue[] GetAll()
        {
            return _rwLock.Read(() => _values.Values.ToArray());
        }

        public void Remove(TKey key)
        {
            _rwLock.Write(()=>_values.Remove(key));
        }

        public void ClearAll()
        {
            _rwLock.Write(()=> _values.Clear());
        }

        public bool WithValue(TKey key, Action<TValue> callback)
        {
            if (Has(key))
            {
                callback(this[key]);
                return true;
            }
            return false;
        }

        public IDictionary<TKey, TValue> ToDictionary()
        {
            return _rwLock.Read(()=> new Dictionary<TKey, TValue>(_values));
        }
    }
}