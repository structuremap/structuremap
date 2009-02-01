using System;
using System.Collections;
using System.Web;
using System.Web.SessionState;
using StructureMap.Attributes;

namespace StructureMap.Pipeline
{
    public class HttpContextBuildPolicy : CacheInterceptor
    {
        public static readonly string ITEM_NAME = "STRUCTUREMAP-INSTANCES";

        public static bool HasContext()
        {
            return HttpContext.Current != null;
        }

        public static void DisposeAndClearAll()
        {
            new HttpContextBuildPolicy().findCache().DisposeAndClear();
        }

        protected override InstanceCache findCache()
        {
            IDictionary items = findHttpDictionary();

            if (!items.Contains(ITEM_NAME))
            {
                lock (items.SyncRoot)
                {
                    if (!items.Contains(ITEM_NAME))
                    {
                        InstanceCache cache = buildNewCache();
                        items.Add(ITEM_NAME, cache);

                        return cache;
                    }
                }
            }

            return (InstanceCache) items[ITEM_NAME];
        }

        protected virtual IDictionary findHttpDictionary()
        {
            return HttpContext.Current.Items;
        }

        protected override CacheInterceptor clone()
        {
            return this;
        }

        public override string ToString()
        {
            return InstanceScope.HttpContext.ToString();
        }
    }

    public class HttpSessionBuildPolicy : HttpContextBuildPolicy
    {
        protected override IDictionary findHttpDictionary()
        {
            return new SessionWrapper(HttpContext.Current.Session);
        }

        public override string ToString()
        {
            return InstanceScope.HttpSession.ToString();
        }
    }

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

        public int Count
        {
            get { return _session.Count; }
        }

        public object SyncRoot
        {
            get { return _session.SyncRoot; }
        }

        public bool IsSynchronized
        {
            get { return _session.IsSynchronized; }
        }

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

        public object this[object key]
        {
            get { return _session[key.ToString()]; }
            set { _session[key.ToString()] = value; }
        }

        public ICollection Keys
        {
            get { return _session.Keys; }
        }

        public ICollection Values
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { return _session.IsReadOnly; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        #endregion
    }
}
