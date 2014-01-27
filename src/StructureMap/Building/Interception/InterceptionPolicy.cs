using System;
using System.Collections.Generic;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Building.Interception
{
    public class InterceptionPolicy<T> : IInterceptorPolicy
    {
        private readonly IInterceptor _interceptor;

        public InterceptionPolicy(IInterceptor interceptor, Func<Instance, bool> filter = null)
        {
            if (!interceptor.Accepts.CanBeCastTo<T>())
            {
                throw new ArgumentOutOfRangeException("The accepts type of {0} cannot be cast to {1}".ToFormat(interceptor.Accepts.GetFullName(), typeof(T).GetFullName()));
            }

            _interceptor = interceptor;
            Filter = filter;
        }

        public Func<Instance, bool> Filter { get; set; }

        public string Description
        {
            get
            {
                return
                    "Apply interceptor '{0}' to all concrete types that can be cast to {1}".ToFormat(
                        _interceptor.Description, typeof (T).GetFullName());
            }
        }

        // TODO -- need to take in PluginType.
        // Decorators only apply when the PluginType == typeof(T)
        // activators are easier
        public IEnumerable<IInterceptor> DetermineInterceptors(Instance instance)
        {
            if (instance.ReturnedType.CanBeCastTo<T>() && (Filter ?? (i => true))(instance))
            {
                yield return _interceptor;
            }
        }

        protected bool Equals(InterceptionPolicy<T> other)
        {
            return Equals(_interceptor, other._interceptor);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((InterceptionPolicy<T>) obj);
        }

        public override int GetHashCode()
        {
            return (_interceptor != null ? _interceptor.GetHashCode() : 0);
        }
    }
}