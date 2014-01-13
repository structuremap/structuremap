using System.Collections;
using System.Reflection;
using System.Web;
using StructureMap.Pipeline;

namespace StructureMap.Web.Pipeline
{
    public class HttpContextLifecycle : LifecycleBase
    {
        public static readonly string ITEM_NAME = string.Format("STRUCTUREMAP-INSTANCES-{0}", Assembly.GetExecutingAssembly().GetName().Version);


        public override void EjectAll(ILifecycleContext context)
        {
            FindCache(context).DisposeAndClear();
        }

        public override IObjectCache FindCache(ILifecycleContext context)
        {
            IDictionary items = findHttpDictionary();

            if (!items.Contains(ITEM_NAME))
            {
                lock (items.SyncRoot)
                {
                    if (!items.Contains(ITEM_NAME))
                    {
                        var cache = new LifecycleObjectCache();
                        items.Add(ITEM_NAME, cache);

                        return cache;
                    }
                }
            }

            return (IObjectCache) items[ITEM_NAME];
        }

        public static bool HasContext()
        {
            return HttpContext.Current != null;
        }

        /// <summary>
        /// Remove and dispose all objects scoped by HttpContext.  Call this method at the *end* of an Http request to clean up resources
        /// </summary>
        public static void DisposeAndClearAll()
        {
            new HttpContextLifecycle().FindCache(null).DisposeAndClear();
        }


        protected virtual IDictionary findHttpDictionary()
        {
            // TODO -- going to suck, but let's try to get a UT on this thing
            if (!HasContext())
            {
                throw new StructureMapException("You cannot use the HttpContextLifecycle outside of a web request. Try the HybridLifecycle instead.");
                
            }

            return HttpContext.Current.Items;
        }
    }
}