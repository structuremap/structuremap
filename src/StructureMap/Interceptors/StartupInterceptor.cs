using System;

namespace StructureMap.Interceptors
{
    public class StartupInterceptor<T> : InstanceInterceptor
    {
        private readonly Action<IContext, T> _handler;

        public StartupInterceptor(Action<IContext, T> handler)
        {
            _handler = handler;
        }

        #region InstanceInterceptor Members

        public object Process(object target, IContext context)
        {
            _handler(context, (T) target);
            return target;
        }

        #endregion
    }
}