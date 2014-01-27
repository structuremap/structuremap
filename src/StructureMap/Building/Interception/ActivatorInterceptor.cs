using System;
using System.Linq;
using System.Linq.Expressions;
using StructureMap.TypeRules;

namespace StructureMap.Building.Interception
{
    public class ActivatorInterceptor<T> : IInterceptor
    {
        private readonly LambdaExpression _action;
        private readonly string _description;

        public ActivatorInterceptor(Expression<Action<T>> action, string description = null)
        {
            _action = action;
            _description = description;
        }

        public ActivatorInterceptor(Expression<Action<IContext, T>> action, string description = null)
        {
            _action = action;
            _description = description;
        }

        public IInterceptorPolicy ToPolicy()
        {
            return new InterceptorPolicy<T>(this);
        }

        public string Description
        {
            get
            {
                var bodyDescription = _description ?? _action
                    .ReplaceParameter(Accepts, Expression.Parameter(Accepts, Accepts.Name))
                    .ReplaceParameter(typeof(IContext), Expression.Parameter(typeof(IContext), "IContext"))
                    .Body.ToString();

                return "ActivatorInterceptor of {0}: {1}".ToFormat(typeof (T).GetFullName(), bodyDescription);
            }
        }

        public InterceptorRole Role
        {
            get
            {
                return InterceptorRole.Activates;
            }
        }

        public Expression ToExpression(Policies policies, ParameterExpression context, ParameterExpression variable)
        {
            return _action
                .ReplaceParameter(Accepts, variable)
                .ReplaceParameter(typeof (IContext), context)
                .Body;
        }

        public Type Accepts { get { return typeof (T); } }
        public Type Returns { get { return typeof(T); } }

        protected bool Equals(ActivatorInterceptor<T> other)
        {
            return Equals(_action, other._action);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ActivatorInterceptor<T>) obj);
        }

        public override int GetHashCode()
        {
            return (_action != null ? _action.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return "Interceptor of {0}: {1}".ToFormat(typeof (T).GetFullName(), Description);
        }
    }
}