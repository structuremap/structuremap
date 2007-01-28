using System;
using System.Collections;

namespace StructureMap.Interceptors
{
    [Pluggable("ThreadLocal")]
    public class ThreadLocalStorageInterceptor : CacheInterceptor
    {
        [ThreadStatic] private static Hashtable _instances = new Hashtable();

        private static object _lock = new object();

        public ThreadLocalStorageInterceptor() : base()
        {
        }

        private void guaranteeHashExists()
        {
            if (_instances == null)
            {
                lock (_lock)
                {
                    if (_instances == null)
                    {
                        _instances = new Hashtable();
                    }
                }
            }
        }


        private string getKey(string instanceKey)
        {
            return string.Format("{0}:{1}", InnerInstanceFactory.PluginType.AssemblyQualifiedName, instanceKey);
        }

        protected override void cache(string instanceKey, object instance)
        {
            _instances.Add(getKey(instanceKey), instance);
        }

        protected override bool isCached(string instanceKey)
        {
            guaranteeHashExists();
            return _instances.ContainsKey(getKey(instanceKey));
        }

        protected override object getInstance(string instanceKey)
        {
            return _instances[getKey(instanceKey)];
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }
}