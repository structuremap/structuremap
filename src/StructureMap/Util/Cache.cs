using System;
using System.Collections;
using System.Collections.Generic;

namespace StructureMap.Util
{
    // TODO -- at least use reader/writer locks
    [Obsolete("Replace this with the one from FubuCore")]
    [Serializable] // TODO -- at a minimum, use ReaderWriterLockSlim
    public class Cache<TKey, TValue> : IEnumerable<TValue> where TValue : class
    {
        private readonly object _locker = new object();
        private readonly IDictionary<TKey, TValue> _values;

        private Func<TValue, TKey> _getKey = delegate { throw new NotImplementedException(); };

        private Func<TKey, TValue> _onMissing = delegate(TKey key)
        {
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


        /// <summary>
        ///   Guarantees that the Cache has the default value for a given key.
        ///   If it does not already exist, it's created.
        /// </summary>
        /// <param name = "key"></param>
        public void FillDefault(TKey key)
        {
            Fill(key, _onMissing(key));
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
            get { return _values.Count; }
        }

        public TValue First
        {
            get
            {
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
                if (!_values.ContainsKey(key))
                {
                    lock (_locker)
                    {
                        if (!_values.ContainsKey(key))
                        {
                            var value = _onMissing(key);
                            //Check to make sure that the onMissing didn't cache this already
                            if (!_values.ContainsKey(key))
                                _values.Add(key, value);
                        }
                    }
                }

                return _values[key];
            }
            set
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TValue>) this).GetEnumerator();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _values.Values.GetEnumerator();
        }

        public void Fill(TKey key, TValue value)
        {
            if (_values.ContainsKey(key))
            {
                return;
            }

            _values.Add(key, value);
        }

        public bool TryRetrieve(TKey key, out TValue value)
        {
            value = default(TValue);

            if (_values.ContainsKey(key))
            {
                value = _values[key];
                return true;
            }

            return false;
        }

        public void Each(Action<TValue> action)
        {
            lock (_locker)
            {
                foreach (var pair in _values)
                {
                    action(pair.Value);
                }
            }
        }

        public void Each(Action<TKey, TValue> action)
        {
            foreach (var pair in _values)
            {
                action(pair.Key, pair.Value);
            }
        }

        public bool Has(TKey key)
        {
            return _values.ContainsKey(key);
        }

        public bool Exists(Predicate<TValue> predicate)
        {
            var returnValue = false;

            Each(delegate(TValue value) { returnValue |= predicate(value); });

            return returnValue;
        }

        public TValue Find(Predicate<TValue> predicate)
        {
            foreach (var pair in _values)
            {
                if (predicate(pair.Value))
                {
                    return pair.Value;
                }
            }

            return null;
        }

        public TValue[] GetAll()
        {
            var returnValue = new TValue[Count];
            _values.Values.CopyTo(returnValue, 0);

            return returnValue;
        }

        public void Remove(TKey key)
        {
            if (_values.ContainsKey(key))
            {
                _values.Remove(key);
            }
        }

        public void Clear()
        {
            _values.Clear();
        }

        public Cache<TKey, TValue> Clone()
        {
            var clone = new Cache<TKey, TValue>(_onMissing);
            _values.Each(pair => clone[pair.Key] = pair.Value);

            return clone;
        }

        public void WithValue(TKey key, Action<TValue> action)
        {
            if (_values.ContainsKey(key))
            {
                action(this[key]);
            }
        }
    }
}