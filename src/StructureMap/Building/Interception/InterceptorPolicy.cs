using System;
using System.Collections.Generic;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Building.Interception
{
    public class InterceptorPolicy<T> : IInterceptorPolicy
    {
        private readonly IInterceptor _interceptor;
        private Func<Instance, bool> _filter;

        public InterceptorPolicy(IInterceptor interceptor, Func<Instance, bool> filter = null)
        {
            if (!interceptor.Accepts.CanBeCastTo<T>())
            {
                throw new ArgumentOutOfRangeException(
                    "The accepts type of {0} cannot be cast to {1}".ToFormat(interceptor.Accepts.GetFullName(),
                        typeof (T).GetFullName()));
            }

            _interceptor = interceptor;
            Filter = filter;
        }

        public Func<Instance, bool> Filter
        {
            get { return _filter ?? (i => true); }
            set { _filter = value; }
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

        public IEnumerable<IInterceptor> DetermineInterceptors(Type pluginType, Instance instance)
        {
            if (!Filter(instance))
            {
                yield break;
            }

            if (_interceptor.Role == InterceptorRole.Decorates && typeof (T) == pluginType)
            {
                yield return _interceptor;
            }
            else if (_interceptor.Role == InterceptorRole.Activates && instance.ReturnedType.CanBeCastTo<T>())
            {
                yield return _interceptor;
            }
        }

        protected bool Equals(InterceptorPolicy<T> other)
        {
            return Equals(_interceptor, other._interceptor);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((InterceptorPolicy<T>) obj);
        }

        public override int GetHashCode()
        {
            return (_interceptor != null ? _interceptor.GetHashCode() : 0);
        }
    }
}