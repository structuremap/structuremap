using System;
using StructureMap.Building.Interception;
using StructureMap.Pipeline;

namespace StructureMap
{
    internal class InterceptionPolicy : IInstancePolicy
    {
        private readonly IInterceptorPolicy _inner;

        public InterceptionPolicy(IInterceptorPolicy inner)
        {
            _inner = inner;
        }

        public void Apply(Type pluginType, Instance instance)
        {
            _inner.DetermineInterceptors(pluginType, instance).Each(instance.AddInterceptor);
        }

        public IInterceptorPolicy Inner
        {
            get { return _inner; }
        }
    }
}