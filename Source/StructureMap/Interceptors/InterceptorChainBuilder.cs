using StructureMap.Attributes;
using StructureMap.Graph;

namespace StructureMap.Interceptors
{
    public class InterceptorChainBuilder : IInterceptorChainBuilder
    {
        public InterceptorChainBuilder()
        {
        }

        public InterceptionChain Build(InstanceScope scope)
        {
            InterceptionChain returnValue = new InterceptionChain();

            switch (scope)
            {
                case (InstanceScope.HttpContext):
                    returnValue.AddInterceptor(new HttpContextItemInterceptor());
                    break;

                case (InstanceScope.Hybrid):
                    returnValue.AddInterceptor(new HybridCacheInterceptor());
                    break;

                case (InstanceScope.Singleton):
                    returnValue.AddInterceptor(new SingletonInterceptor());
                    break;

                case (InstanceScope.ThreadLocal):
                    returnValue.AddInterceptor(new ThreadLocalStorageInterceptor());
                    break;
            }


            return returnValue;
        }
    }
}