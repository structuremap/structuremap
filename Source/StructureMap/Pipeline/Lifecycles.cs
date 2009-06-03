using System;
using StructureMap.Attributes;

namespace StructureMap.Pipeline
{
    public static class Lifecycles
    {
        public static ILifecycle GetLifecycle(InstanceScope scope)
        {
            switch (scope)
            {
                case InstanceScope.PerRequest:
                    return null;

                case InstanceScope.Singleton:
                    return new SingletonLifecycle();

                case InstanceScope.HttpContext:
                    return new HttpContextLifecycle();

                case InstanceScope.ThreadLocal:
                    return new ThreadLocalStorageLifecycle();

                case InstanceScope.Hybrid:
                    return new HybridLifecycle();

                case InstanceScope.HttpSession:
                    return new HttpSessionLifecycle();

                case InstanceScope.HybridHttpSession:
                    return new HybridSessionLifecycle();
            }
            
            throw new ArgumentOutOfRangeException("scope");
        }
    }
}