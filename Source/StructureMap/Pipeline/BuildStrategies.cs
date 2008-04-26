using System;

namespace StructureMap.Pipeline
{
    public interface IBuildPolicy
    {
        object Build(IInstanceCreator instanceCreator, Type pluginType, Instance instance);
        IBuildPolicy Clone();
    }

    public class BuildPolicy : IBuildPolicy
    {
        #region IBuildPolicy Members

        public object Build(IInstanceCreator instanceCreator, Type pluginType, Instance instance)
        {
            return instance.Build(pluginType, instanceCreator);
        }

        public IBuildPolicy Clone()
        {
            return this;
        }

        #endregion
    }

    [PluginFamily]
    public interface IInstanceInterceptor : IBuildPolicy
    {
        IBuildPolicy InnerPolicy { get; set; }
    }

    public abstract class CacheInterceptor : IInstanceInterceptor
    {
        private readonly object _locker = new object();
        private IBuildPolicy _innerPolicy = new BuildPolicy();

        #region IInstanceInterceptor Members

        public IBuildPolicy InnerPolicy
        {
            get { return _innerPolicy; }
            set { _innerPolicy = value; }
        }


        public object Build(IInstanceCreator instanceCreator, Type pluginType, Instance instance)
        {
            if (!isCached(instance.Name, instance.PluginType))
            {
                lock (_locker)
                {
                    if (!isCached(instance.Name, pluginType))
                    {
                        object returnValue = _innerPolicy.Build(instanceCreator, pluginType, instance);
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

        protected abstract CacheInterceptor clone();

        #endregion

        protected abstract void storeInCache(string instanceKey, Type pluginType, object instance);
        protected abstract bool isCached(string instanceKey, Type pluginType);
        protected abstract object retrieveFromCache(string instanceKey, Type pluginType);
    }

    public class HybridBuildPolicy : IInstanceInterceptor
    {
        private readonly IInstanceInterceptor _innerInterceptor;


        public HybridBuildPolicy()
        {
            _innerInterceptor = HttpContextBuildPolicy.HasContext()
                                    ? (IInstanceInterceptor) new HttpContextBuildPolicy()
                                    : new ThreadLocalStoragePolicy();
        }

        #region IInstanceInterceptor Members

        public IBuildPolicy InnerPolicy
        {
            get { return _innerInterceptor.InnerPolicy; }
            set { _innerInterceptor.InnerPolicy = value; }
        }

        public object Build(IInstanceCreator instanceCreator, Type pluginType, Instance instance)
        {
            return _innerInterceptor.Build(instanceCreator, pluginType, instance);
        }

        public IBuildPolicy Clone()
        {
            HybridBuildPolicy policy = new HybridBuildPolicy();
            policy.InnerPolicy = InnerPolicy.Clone();

            return policy;
        }

        #endregion
    }
}