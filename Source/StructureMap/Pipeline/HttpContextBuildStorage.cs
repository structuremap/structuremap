using System.Collections;
using System.Web;

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
            IDictionary items = HttpContext.Current.Items;

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

        protected override CacheInterceptor clone()
        {
            return this;
        }
    }
}