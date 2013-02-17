using System;

namespace StructureMap.Pipeline
{
    // TODO -- make all of these flyweights
    public static class Lifecycles
    {
        public static readonly ILifecycle Transient = new TransientLifecycle();
        public static readonly ILifecycle Singleton = new SingletonLifecycle();
        public static readonly ILifecycle Unique = new UniquePerRequestLifecycle();

        public static ILifecycle GetLifecycle(InstanceScope scope)
        {
            switch (scope)
            {
                case InstanceScope.PerRequest:
                    return Transient;

                case InstanceScope.Singleton:
                    return Singleton;

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

                case InstanceScope.Unique:
                    return Unique;

                case InstanceScope.Transient:
                    return Transient;
            }

            throw new ArgumentOutOfRangeException("scope");
        }
    }
}