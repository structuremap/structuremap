using System;

namespace StructureMap.Pipeline
{
    public abstract class CacheInterceptor : IBuildInterceptor
    {
        private readonly object _locker = new object();
        private IBuildPolicy _innerPolicy = new BuildPolicy();

        #region IBuildInterceptor Members

        public IBuildPolicy InnerPolicy
        {
            get { return _innerPolicy; }
            set { _innerPolicy = value; }
        }


        public object Build(IBuildSession buildSession, Type pluginType, Instance instance)
        {
            if (!isCached(instance.Name, instance.PluginType))
            {
                lock (_locker)
                {
                    if (!isCached(instance.Name, pluginType))
                    {
                        object returnValue = _innerPolicy.Build(buildSession, pluginType, instance);
                        storeInCache(instance.Name, pluginType, returnValue);
                    }
                }
            }

            return retrieveFromCache(instance.Name, pluginType);
        }

        public IBuildPolicy Clone()
        {
            CacheInterceptor clonedCache = clone();
            clonedCache.InnerPolicy = _innerPolicy.Clone();

            return clonedCache;
        }

        #endregion

        protected abstract CacheInterceptor clone();

        protected abstract void storeInCache(string instanceKey, Type pluginType, object instance);
        protected abstract bool isCached(string instanceKey, Type pluginType);
        protected abstract object retrieveFromCache(string instanceKey, Type pluginType);
    }
}