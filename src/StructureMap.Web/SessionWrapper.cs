using System;
using System.Collections;
using System.Web.SessionState;

namespace StructureMap.Web
{
    public class SessionWrapper : IDictionary
    {
        private readonly HttpSessionState _session;

        public SessionWrapper(HttpSessionState session)
        {
            _session = session;
        }

        #region IDictionary Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            _session.CopyTo(array, index);
        }

        public int Count { get { return _session.Count; } }

        public object SyncRoot { get { return _session.SyncRoot; } }

        public bool IsSynchronized { get { return _session.IsSynchronized; } }

        public bool Contains(object key)
        {
            return _session[key.ToString()] != null;
        }

        public void Add(object key, object value)
        {
            _session.Add(key.ToString(), value);
        }

        public void Clear()
        {
            _session.Clear();
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Remove(object key)
        {
            _session.Remove(key.ToString());
        }

        public object this[object key] { get { return _session[key.ToString()]; } set { _session[key.ToString()] = value; } }

        public ICollection Keys { get { return _session.Keys; } }

        public ICollection Values { get { throw new NotImplementedException(); } }

        public bool IsReadOnly { get { return _session.IsReadOnly; } }

        public bool IsFixedSize { get { return false; } }

        #endregion
    }
}