using System;

namespace StructureMap.Interceptors
{
    [Obsolete]
    public class EnrichmentInterceptor<T> : InstanceInterceptor
    {
        private readonly Func<IBuildSession, T, T> _handler;

        public EnrichmentInterceptor(Func<IBuildSession, T, T> handler)
        {
            _handler = handler;
        }

        public object Process(object target, IBuildSession session)
        {
            return _handler(session, (T) target);
        }
    }
}