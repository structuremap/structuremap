using System.Collections;
using System.Reflection;
using System.Web;

namespace StructureMap.Pipeline
{
    public class HttpContextLifecycle : ILifecycle
    {
        public static readonly string ITEM_NAME = string.Format("STRUCTUREMAP-INSTANCES-{0}", Assembly.GetExecutingAssembly().GetName().Version);


        public void EjectAll()
        {
            FindCache().DisposeAndClear();
        }

        public IObjectCache FindCache()
        {
            IDictionary items = findHttpDictionary();

            if (!items.Contains(ITEM_NAME))
            {
                lock (items.SyncRoot)
                {
                    if (!items.Contains(ITEM_NAME))
                    {
                        var cache = new MainObjectCache();
                        items.Add(ITEM_NAME, cache);

                        return cache;
                    }
                }
            }

            return (IObjectCache) items[ITEM_NAME];
        }

        public string Scope { get { return InstanceScope.HttpContext.ToString(); } }

        public static bool HasContext()
        {
            return HttpContext.Current != null;
        }

        /// <summary>
        /// Remove and dispose all objects scoped by HttpContext.  Call this method at the *end* of an Http request to clean up resources
        /// </summary>
        public static void DisposeAndClearAll()
        {
            new HttpContextLifecycle().FindCache().DisposeAndClear();
        }


        protected virtual IDictionary findHttpDictionary()
        {
            if (!HasContext())
                throw new StructureMapException(309);

            return HttpContext.Current.Items;
        }
    }
}