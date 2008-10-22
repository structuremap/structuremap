using System;

namespace StructureMap.Pipeline
{
    public class HybridBuildPolicy : IBuildInterceptor
    {
        private IBuildPolicy _innerPolicy;

        private IBuildInterceptor interceptor
        {
            get
            {
                return HttpContextBuildPolicy.HasContext()
                                    ? (IBuildInterceptor)new HttpContextBuildPolicy(){InnerPolicy = _innerPolicy}
                                    : new ThreadLocalStoragePolicy(){InnerPolicy = _innerPolicy};
            }
        }

        #region IBuildInterceptor Members

        public IBuildPolicy InnerPolicy
        {
            get { return _innerPolicy; }
            set { _innerPolicy = value; }
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