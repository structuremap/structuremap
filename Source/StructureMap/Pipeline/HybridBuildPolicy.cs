using System;

namespace StructureMap.Pipeline
{
    public class HybridBuildPolicy : IBuildInterceptor
    {
        private readonly IBuildInterceptor _innerInterceptor;


        public HybridBuildPolicy()
        {
            _innerInterceptor = HttpContextBuildPolicy.HasContext()
                                    ? (IBuildInterceptor) new HttpContextBuildPolicy()
                                    : new ThreadLocalStoragePolicy();
        }

        #region IBuildInterceptor Members

        public IBuildPolicy InnerPolicy
        {
            get { return _innerInterceptor.InnerPolicy; }
            set { _innerInterceptor.InnerPolicy = value; }
        }

        public object Build(BuildSession buildSession, Type pluginType, Instance instance)
        {
            return _innerInterceptor.Build(buildSession, pluginType, instance);
        }

        public IBuildPolicy Clone()
        {
            var policy = new HybridBuildPolicy();
            policy.InnerPolicy = InnerPolicy.Clone();

            return policy;
        }

        #endregion
    }
}