using System;
using StructureMap.Util;

namespace StructureMap.Pipeline
{
    public static class Lifecycles
    {
        private static readonly LightweightCache<Type, ILifecycle> _lifecycles =
            new LightweightCache<Type, ILifecycle>(type => (ILifecycle) Activator.CreateInstance(type));


        public static readonly ILifecycle Transient = Register<TransientLifecycle>();
        public static readonly ILifecycle Singleton = Register<SingletonLifecycle>();
        public static readonly ILifecycle Unique = Register<UniquePerRequestLifecycle>();

        public static readonly ILifecycle ThreadLocal = Register<ThreadLocalStorageLifecycle>();
        public static readonly ILifecycle Container = Register<ContainerLifecycle>();


        public static ILifecycle Register<T>() where T : ILifecycle, new()
        {
            var lifecycle = new T();
            _lifecycles[typeof (T)] = lifecycle;
            return lifecycle;
        }

        public static ILifecycle Get<T>() where T : ILifecycle, new()
        {
            return _lifecycles[typeof (T)];
        }

    }
}