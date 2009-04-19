using System.Collections;
using System.Web;
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

        protected override ObjectCache findCache()
        {
            IDictionary items = findHttpDictionary();

            if (!items.Contains(ITEM_NAME))
            {
                lock (items.SyncRoot)
                {
                    if (!items.Contains(ITEM_NAME))
                    {
                        ObjectCache cache = buildNewCache();
                        items.Add(ITEM_NAME, cache);

                        return cache;
                    }
                }
            }

            return (ObjectCache) items[ITEM_NAME];
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
}
