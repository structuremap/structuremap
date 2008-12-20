using System;
using StructureMap.Util;

namespace StructureMap.Pipeline
{
    public class InstanceCache : Cache<InstanceKey, object>
    {
        public InstanceCache(IBuildPolicy innerPolicy) : base(key => innerPolicy.Build(key.Session, key.PluginType, key.Instance))
        {
        }
    }

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

        protected InstanceCache buildNewCache()
        {
            return new InstanceCache(_innerPolicy);
        }

        protected abstract InstanceCache findCache();

        public object Build(BuildSession buildSession, Type pluginType, Instance instance)
        {
            var key = new InstanceKey{Instance = instance, PluginType = pluginType, Session = buildSession};
            return findCache()[key];
        }

        public IBuildPolicy Clone()
        {
            CacheInterceptor clonedCache = clone();
            clonedCache.InnerPolicy = _innerPolicy.Clone();

            return clonedCache;
        }

        #endregion

        protected abstract CacheInterceptor clone();

        public override string ToString()
        {
            return GetType().FullName + " / " + _innerPolicy;
        }
    }
}