using System;

namespace StructureMap.Interceptors
{
    public class StartupInterceptor<T> : InstanceInterceptor
    {
        private readonly Action<T> _handler;

        public StartupInterceptor(Action<T> handler)
        {
            _handler = handler;
        }

        #region InstanceInterceptor Members

        public object Process(object target)
        {
            _handler((T) target);
            return target;
        }

        #endregion
    }
}