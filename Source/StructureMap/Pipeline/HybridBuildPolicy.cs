using System;

namespace StructureMap.Pipeline
{
    public class HybridBuildPolicy : IBuildInterceptor
    {
        private readonly IBuildInterceptor _http;
        private readonly IBuildInterceptor _threadLocal;

        public HybridBuildPolicy()
        {
            _http = new HttpContextBuildPolicy();
            _threadLocal = new ThreadLocalStoragePolicy();
        }

        private IBuildPolicy _innerPolicy;
        

        private IBuildInterceptor interceptor
        {
            get
            {
                return HttpContextBuildPolicy.HasContext()
                                    ? _http
                                    : _threadLocal;
            }
        }

        #region IBuildInterceptor Members

        public IBuildPolicy InnerPolicy
        {
            get { return _innerPolicy; }
            set
            {
                _http.InnerPolicy = value;
                _threadLocal.InnerPolicy = value;
                _innerPolicy = value;
            }
        }

        public object Build(BuildSession buildSession, Type pluginType, Instance instance)
        {
            return interceptor.Build(buildSession, pluginType, instance);
        }

        public IBuildPolicy Clone()
        {
            var policy = new HybridBuildPolicy();
            policy.InnerPolicy = _innerPolicy.Clone();

            return policy;
        }

        #endregion
    }
}