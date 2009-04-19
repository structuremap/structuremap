using System.Collections;
using System.Web;

namespace StructureMap.Pipeline
{
    public class HttpContextLifecycle : ILifecycle
    {
        public static readonly string ITEM_NAME = "STRUCTUREMAP-INSTANCES";


        public static bool HasContext()
        {
            return HttpContext.Current != null;
        }

        public static void DisposeAndClearAll()
        {
            new HttpContextLifecycle().FindCache().DisposeAndClear();
        }

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
                        MainObjectCache cache = new MainObjectCache();
                        items.Add(ITEM_NAME, cache);

                        return cache;
                    }
                }
            }

            return (IObjectCache)items[ITEM_NAME];
        }


        protected virtual IDictionary findHttpDictionary()
        {
            return HttpContext.Current.Items;
        }
    }
}