using System;
using System.Collections;
using System.Web;
using System.Web.SessionState;

namespace StructureMap.Pipeline
{
    public class HttpContextBuildPolicy : CacheInterceptor
    {
        public static readonly string ITEM_NAME = "STRUCTUREMAP-INSTANCES";

        public static bool HasContext()
        {
            return HttpContext.Current != null;
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
    }

    public class HttpSessionBuildPolicy : HttpContextBuildPolicy
    {
        protected override IDictionary findHttpDictionary()
        {
            return new SessionWrapper(HttpContext.Current.Session);
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
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { return _session.SyncRoot; }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
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
            throw new NotImplementedException();
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        public object this[object key]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public ICollection Keys
        {
            get { throw new NotImplementedException(); }
        }

        public ICollection Values
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}