using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Pipeline;

namespace StructureMap.Building.Interception
{
    public class InterceptorPolicies
    {
        private readonly IList<IInterceptorPolicy> _policies = new List<IInterceptorPolicy>();

        public void Add(IInterceptorPolicy policy)
        {
            _policies.Fill(policy);
        }

        public void Add<T>(IInterceptor interceptor, Func<Instance, bool> filter = null)
        {
            Add(new InterceptorPolicy<T>(interceptor, filter));
        }

        public IList<IInterceptorPolicy> Policies
        {
            get { return _policies; }
        }

        public IEnumerable<IInterceptor> SelectInterceptors(Type pluginType, Instance instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            return _policies.SelectMany(x => x.DetermineInterceptors(pluginType, instance)).ToArray();
        }
    }
}