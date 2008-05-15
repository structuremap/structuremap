using System;
using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public class ThreadLocalStoragePolicy : CacheInterceptor
    {
        [ThreadStatic] private static Dictionary<string, object> _instances;
        private object _locker = new object();

        private void guaranteeHashExists()
        {
            if (_instances == null)
            {
                lock (_locker)
                {
                    if (_instances == null)
                    {
                        _instances = new Dictionary<string, object>();
                    }
                }
            }
        }

        protected override void storeInCache(string instanceKey, Type pluginType, object instance)
        {
            _instances.Add(getKey(instanceKey, pluginType), instance);
        }

        protected override bool isCached(string instanceKey, Type pluginType)
        {
            guaranteeHashExists();
            return _instances.ContainsKey(getKey(instanceKey, pluginType));
        }

        protected override object retrieveFromCache(string instanceKey, Type pluginType)
        {
            return _instances[getKey(instanceKey, pluginType)];
        }

        private string getKey(string instanceKey, Type pluginType)
        {
            return string.Format("{0}:{1}", pluginType.AssemblyQualifiedName, instanceKey);
        }

        protected override CacheInterceptor clone()
        {
            return this;
        }
    }
}