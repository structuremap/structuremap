using System;

namespace StructureMap.Pipeline
{
    // TODO -- make it possible to register Lifecycles
    public static class Lifecycles
    {
        public static readonly ILifecycle Transient = new TransientLifecycle();
        public static readonly ILifecycle Singleton = new SingletonLifecycle();
        public static readonly ILifecycle Unique = new UniquePerRequestLifecycle();
        public static readonly ILifecycle HttpContext = new HttpContextLifecycle();
        public static readonly ILifecycle ThreadLocal = new ThreadLocalStorageLifecycle();
        public static readonly HybridLifecycle Hybrid = new HybridLifecycle();
        public static readonly HttpSessionLifecycle HttpSession = new HttpSessionLifecycle();
        public static readonly HybridSessionLifecycle HybridSession = new HybridSessionLifecycle();

        public static ILifecycle GetLifecycle(string scope)
        {
            switch (scope)
            {
                case InstanceScope.Singleton:
                    return Singleton;

                case InstanceScope.HttpContext:
                    return HttpContext;

                case InstanceScope.ThreadLocal:
                    return ThreadLocal;

                case InstanceScope.Hybrid:
                    return Hybrid;

                case InstanceScope.HttpSession:
                    return HttpSession;

                case InstanceScope.HybridHttpSession:
                    return HybridSession;

                case InstanceScope.Unique:
                    return Unique;

                case InstanceScope.Transient:
                    return Transient;
            }

            throw new ArgumentOutOfRangeException("scope");
        }
    }
}