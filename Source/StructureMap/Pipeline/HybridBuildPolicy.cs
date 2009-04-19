using System;
using StructureMap.Attributes;

namespace StructureMap.Pipeline
{
    public abstract class HttpBuildPolicyBase<HTTP, NONHTTP> : IBuildInterceptor 
        where HTTP : IBuildInterceptor, new()
        where NONHTTP : IBuildInterceptor, new()
    {
        private readonly IBuildInterceptor _http;
        private readonly IBuildInterceptor _nonHttp;

        public HttpBuildPolicyBase()
        {
            _http = new HTTP();
            _nonHttp = new NONHTTP();
        }


        private IBuildPolicy _innerPolicy;
        

        private IBuildInterceptor interceptor
        {
            get
            {
                return HttpContextBuildPolicy.HasContext()
                                    ? _http
                                    : _nonHttp;
            }
        }

        public IBuildPolicy InnerPolicy
        {
            get { return _innerPolicy; }
            set
            {
                _http.InnerPolicy = value;
                _nonHttp.InnerPolicy = value;
                _innerPolicy = value;
            }
        }

        public object Build(BuildSession buildSession, Type pluginType, Instance instance)
        {
            return interceptor.Build(buildSession, pluginType, instance);
        }

        public abstract IBuildPolicy Clone();
        public void EjectAll()
        {
            _http.EjectAll();
            _nonHttp.EjectAll();
        }
    }





    public class HybridBuildPolicy : HttpBuildPolicyBase<HttpContextBuildPolicy, ThreadLocalStoragePolicy>
    {
        public override IBuildPolicy Clone()
        {
            return new HybridBuildPolicy(){InnerPolicy = InnerPolicy.Clone()};
        }

        public override string ToString()
        {
            return InstanceScope.Hybrid.ToString();
        }
    }

}