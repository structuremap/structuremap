using System;
using StructureMap.Util;

namespace StructureMap.Pipeline
{
    public class ObjectCache : Cache<InstanceKey, object>
    {
        public ObjectCache(IBuildPolicy innerPolicy) : base(key => innerPolicy.Build(key.Session, key.PluginType, key.Instance))
        {
        }

        public void DisposeAndClear()
        {
            Each(o =>
            {
                IDisposable disposable = o as IDisposable;
                if (disposable != null)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception) { }
                }
            });
            Clear();
        }
    }

    public abstract class CacheInterceptor : IBuildInterceptor
    {
        private IBuildPolicy _innerPolicy = new BuildPolicy();

        #region IBuildInterceptor Members

        public IBuildPolicy InnerPolicy
        {
            get { return _innerPolicy; }
            set { _innerPolicy = value; }
        }

        protected ObjectCache buildNewCache()
        {
            return new ObjectCache(_innerPolicy);
        }

        protected abstract ObjectCache findCache();

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

        public void EjectAll()
        {
            ejectAll();
            _innerPolicy.EjectAll();
        }

        protected virtual void ejectAll()
        {
        }

        #endregion

        protected abstract CacheInterceptor clone();

        public override string ToString()
        {
            return GetType().FullName + " / " + _innerPolicy;
        }
    }
}