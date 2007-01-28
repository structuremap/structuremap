namespace StructureMap.Interceptors
{
    public abstract class CacheInterceptor : InstanceFactoryInterceptor
    {
        public CacheInterceptor() : base()
        {
        }

        public override object GetInstance()
        {
            return GetInstance(DefaultInstanceKey);
        }

        public override object GetInstance(string instanceKey)
        {
            ensureInstanceIsCached(instanceKey);
            return getInstance(instanceKey);
        }

        private void ensureInstanceIsCached(string instanceKey)
        {
            if (!isCached(instanceKey))
            {
                lock (this)
                {
                    if (!isCached(instanceKey))
                    {
                        object instance = InnerInstanceFactory.GetInstance(instanceKey);
                        cache(instanceKey, instance);
                    }
                }
            }
        }

        protected abstract void cache(string instanceKey, object instance);

        protected abstract bool isCached(string instanceKey);

        protected abstract object getInstance(string instanceKey);
    }
}