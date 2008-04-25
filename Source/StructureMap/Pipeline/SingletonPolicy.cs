using System;
using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public class SingletonPolicy : CacheInterceptor
    {
        private readonly Dictionary<string, object> _instances = new Dictionary<string, object>();

        protected override void storeInCache(string instanceKey, Type pluginType, object instance)
        {
            _instances.Add(instanceKey, instance);
        }

        protected override bool isCached(string instanceKey, Type pluginType)
        {
            return _instances.ContainsKey(instanceKey);
        }

        protected override object retrieveFromCache(string instanceKey, Type pluginType)
        {
            return _instances[instanceKey];
        }

        protected override CacheInterceptor clone()
        {
            return new SingletonPolicy();
        }
    }
}