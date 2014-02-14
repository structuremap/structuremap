using System;

namespace StructureMap.Building.Interception
{
    // Tested through integration tests on interception
    public static class InterceptorFactory
    {
        public static IInterceptor ForAction<T>(string description, Action<T> action)
        {
            return new ActivatorInterceptor<T>(x => action(x), description);
        }

        public static IInterceptor ForAction<T>(string description, Action<IContext, T> action)
        {
            return new ActivatorInterceptor<T>((s, x) => action(s, x));
        }

        public static IInterceptor ForFunc<T, TPluginType>(string description, Func<T, TPluginType> func) where T : TPluginType
        {
            return new FuncInterceptor<T, TPluginType>(x => func(x), description);
        }

        public static IInterceptor ForFunc<T, TPluginType>(string description, Func<IContext, T, TPluginType> func) where T : TPluginType
        {
            return new FuncInterceptor<T, TPluginType>((s, x) => func(s, x), description);
        }
    }
}