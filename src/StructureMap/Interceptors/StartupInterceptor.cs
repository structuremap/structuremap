using System;

namespace StructureMap.Interceptors
{
    [Obsolete]
    public class StartupInterceptor<T> : InstanceInterceptor
    {
        private readonly Action<IContext, T> _handler;

        public StartupInterceptor(Action<IContext, T> handler)
        {
            _handler = handler;
        }

        #region InstanceInterceptor Members

        public object Process(object target, IBuildSession session)
        {
            _handler(session, (T) target);
            return target;
        }

        #endregion
    }
}