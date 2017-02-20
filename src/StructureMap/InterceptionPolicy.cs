using System;
using StructureMap.Building.Interception;
using StructureMap.Pipeline;

namespace StructureMap
{
    internal class InterceptionPolicy : IInstancePolicy
    {
        public InterceptionPolicy(IInterceptorPolicy inner)
        {
            Inner = inner;
        }

        public void Apply(Type pluginType, Instance instance)
        {
            Inner.DetermineInterceptors(pluginType, instance).Each(instance.AddInterceptor);
        }

        public IInterceptorPolicy Inner { get; }
    }
}