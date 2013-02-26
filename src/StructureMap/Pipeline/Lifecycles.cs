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

        public static ILifecycle Get<T>() where T: ILifecycle
        {
            return Activator.CreateInstance<T>();
        }
    }
}