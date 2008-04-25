using System;
using System.Web;

namespace StructureMap.Pipeline
{
    public class HttpContextBuildPolicy : CacheInterceptor
    {
        public static bool HasContext()
        {
            return HttpContext.Current != null;
        }

        protected override void storeInCache(string instanceKey, Type pluginType, object instance)
        {
            HttpContext.Current.Items.Add(getKey(instanceKey, pluginType), instance);
        }

        protected override bool isCached(string instanceKey, Type pluginType)
        {
            return HttpContext.Current.Items.Contains(getKey(instanceKey, pluginType));
        }

        protected override object retrieveFromCache(string instanceKey, Type pluginType)
        {
            return HttpContext.Current.Items[getKey(instanceKey, pluginType)];
        }

        private static string getKey(string instanceKey, Type pluginType)
        {
            return string.Format("{0}:{1}", pluginType.AssemblyQualifiedName, instanceKey);
        }

        protected override CacheInterceptor clone()
        {
            return this;
        }
    }
}