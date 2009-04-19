using System;
using System.Collections;
using System.Collections.Generic;

namespace StructureMap.Util
{
    public class Cache<KEY, VALUE> : IEnumerable<VALUE> where VALUE : class
    {
        private readonly object _locker = new object();
        private readonly IDictionary<KEY, VALUE> _values;

        private Func<VALUE, KEY> _getKey = delegate { throw new NotImplementedException(); };

        private Func<KEY, VALUE> _onMissing = delegate(KEY key)
        {
            string message = string.Format("Key '{0}' could not be found", key);
            throw new KeyNotFoundException(message);
        };

        public Cache()
            : this(new Dictionary<KEY, VALUE>())
        {
        }

        public Cache(Func<KEY, VALUE> onMissing)
            : this(new Dictionary<KEY, VALUE>(), onMissing)
        {
        }

        public Cache(IDictionary<KEY, VALUE> dictionary, Func<KEY, VALUE> onMissing)
            : this(dictionary)
        {
            _onMissing = onMissing;
        }

        public Cache(IDictionary<KEY, VALUE> dictionary)
        {
            _values = dictionary;
        }

        public Func<KEY, VALUE> OnMissing
        {
            set { _onMissing = value; }
        }

        public Func<VALUE, KEY> GetKey
        {
            get { return _getKey; }
            set { _getKey = value; }
        }

        public int Count
        {
            get { return _values.Count; }
        }

        public VALUE First
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


        public VALUE this[KEY key]
        {
            get
            {
                if (!_values.ContainsKey(key))
                {
                    lock (_locker)
                    {
                        if (!_values.ContainsKey(key))
                        {
                            VALUE value = _onMissing(key);
                            //Check to make sure that the onMissing didn't cache this already
                            if(!_values.ContainsKey(key))
                            {
                                _values.Add(key, value);
                            }
                        }
                    }
                }

                return _values[key];
            }
            set
            {
                lock (_locker)
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

        #region IEnumerable<VALUE> Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<VALUE>)this).GetEnumerator();
        }

        public IEnumerator<VALUE> GetEnumerator()
        {
            return _values.Values.GetEnumerator();
        }

        #endregion

        public void Fill(KEY key, VALUE value)
        {
            if (_values.ContainsKey(key))
            {
                return;
            }

            _values.Add(key, value);
        }

        public bool TryRetrieve(KEY key, out VALUE value)
        {
            value = default(VALUE);

            if (_values.ContainsKey(key))
            {
                value = _values[key];
                return true;
            }

            return false;
        }

        public void Each(Action<VALUE> action)
        {
            lock (_locker)
            {
                foreach (var pair in _values)
                {
                    action(pair.Value);
                }
            }
        }

        public void Each(Action<KEY, VALUE> action)
        {
            foreach (var pair in _values)
            {
                action(pair.Key, pair.Value);
            }
        }

        public bool Has(KEY key)
        {
            return _values.ContainsKey(key);
        }

        public bool Exists(Predicate<VALUE> predicate)
        {
            bool returnValue = false;

            Each(delegate(VALUE value) { returnValue |= predicate(value); });

            return returnValue;
        }

        public VALUE Find(Predicate<VALUE> predicate)
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

        public VALUE[] GetAll()
        {
            var returnValue = new VALUE[Count];
            _values.Values.CopyTo(returnValue, 0);

            return returnValue;
        }

        public void Remove(KEY key)
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
    }
}
