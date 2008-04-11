using StructureMap.Pipeline;

namespace StructureMap.Interceptors
{
    [Pluggable("Hybrid")]
    public class HybridCacheInterceptor : InstanceFactoryInterceptor
    {
        private InstanceFactoryInterceptor _innerInterceptor;

        public HybridCacheInterceptor() : base()
        {
            if (HttpContextItemInterceptor.HasContext())
            {
                _innerInterceptor = new HttpContextItemInterceptor();
            }
            else
            {
                _innerInterceptor = new ThreadLocalStorageInterceptor();
            }
        }

        public override IInstanceFactory InnerInstanceFactory
        {
            get { return _innerInterceptor.InnerInstanceFactory; }
            set { _innerInterceptor.InnerInstanceFactory = value; }
        }

        public override object GetInstance(string instanceKey)
        {
            return _innerInterceptor.GetInstance(instanceKey);
        }

        public override object GetInstance(IConfiguredInstance instance, IInstanceCreator instanceCreator)
        {
            return _innerInterceptor.GetInstance(instance, instanceCreator);
        }

        public override object GetInstance()
        {
            return _innerInterceptor.GetInstance();
        }

        public override object Clone()
        {
            return new HybridCacheInterceptor();
        }
    }
}