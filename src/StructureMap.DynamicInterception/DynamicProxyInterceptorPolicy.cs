using StructureMap.Building.Interception;
using StructureMap.Pipeline;
using System;
using System.Collections.Generic;

namespace StructureMap.DynamicInterception
{
    public class DynamicProxyInterceptorPolicy : IInterceptorPolicy
    {
        private static readonly Func<Type, Instance, bool> TrueFilter = ((type, instance) => true);

        private readonly Func<Type, Instance, bool> _filter;
        private readonly IInterceptionBehavior[] _interceptionBehaviors;

        public DynamicProxyInterceptorPolicy(Func<Type, Instance, bool> filter, params IInterceptionBehavior[] interceptionBehaviors)
        {
            _filter = filter ?? TrueFilter;
            _interceptionBehaviors = interceptionBehaviors;
        }

        public DynamicProxyInterceptorPolicy(Func<Type, bool> filter, params IInterceptionBehavior[] interceptionBehaviors)
            : this((type, instance) => filter(type), interceptionBehaviors)
        {
        }

        public DynamicProxyInterceptorPolicy(Func<Instance, bool> filter, params IInterceptionBehavior[] interceptionBehaviors)
            : this((type, instance) => filter(instance), interceptionBehaviors)
        {
        }

        public DynamicProxyInterceptorPolicy(params IInterceptionBehavior[] interceptionBehaviors)
            : this(TrueFilter, interceptionBehaviors)
        {
        }

        public IEnumerable<IInterceptor> DetermineInterceptors(Type pluginType, Instance instance)
        {
            if (_filter(pluginType, instance))
            {
                var interceptorType = typeof(DynamicProxyInterceptor<>).MakeGenericType(pluginType);
                var interceptor = (IInterceptor)Activator.CreateInstance(interceptorType, new object[] { _interceptionBehaviors });
                yield return interceptor;
            }
        }

        public string Description
        {
            get
            {
                return "Decorate with dynamic proxy class implementing '{0}' using the following interception behaviors";
            }
        }
    }
}