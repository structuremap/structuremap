using System;
using System.Web;

namespace StructureMap.Pipeline
{
    public class HttpContextBuildPolicy : CacheInterceptor
    {
        private string _prefix = Guid.NewGuid().ToString();

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

        private string getKey(string instanceKey, Type pluginType)
        {
            return string.Format("{0}:{1}:{2}", pluginType.AssemblyQualifiedName, instanceKey, _prefix);
        }

        protected override CacheInterceptor clone()
        {
            return this;
        }
    }
}