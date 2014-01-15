using System;
using System.Collections.Generic;
using StructureMap.TypeRules;

namespace StructureMap.Building.Interception
{
    public class InterceptionPolicy<T> : IInterceptorPolicy
    {
        private readonly IInterceptor _interceptor;

        public InterceptionPolicy(IInterceptor interceptor)
        {
            if (!interceptor.Accepts.CanBeCastTo<T>())
            {
                throw new ArgumentOutOfRangeException("The accepts type of {0} cannot be cast to {1}".ToFormat(interceptor.Accepts.GetFullName(), typeof(T).GetFullName()));
            }

            _interceptor = interceptor;
        }

        public string Description
        {
            get
            {
                return
                    "Apply interceptor '{0}' to all concrete types that can be cast to {1}".ToFormat(
                        _interceptor.Description, typeof (T).GetFullName());
            }
        }

        public IEnumerable<IInterceptor> DetermineInterceptors(Type concreteType)
        {
            if (concreteType.CanBeCastTo<T>())
            {
                yield return _interceptor;
            }
        }
    }
}