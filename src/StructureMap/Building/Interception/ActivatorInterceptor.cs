using System;
using System.Linq;
using System.Linq.Expressions;

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

        public ActivatorInterceptor(Expression<Action<IBuildSession, T>> action, string description = null)
        {
            _action = action;
            _description = description;
        }

        public string Description
        {
            get
            {
                return _description ?? _action
                    .ReplaceParameter(Accepts, Expression.Parameter(Accepts, Accepts.Name))
                    .ReplaceParameter(typeof(IBuildSession), Expression.Parameter(typeof(IBuildSession), "IBuildSession"))
                    .Body.ToString();
            }
        }

        public InterceptorRole Role
        {
            get
            {
                return InterceptorRole.Activates;
            }
        }

        public Expression ToExpression(ParameterExpression session, ParameterExpression variable)
        {
            return _action
                .ReplaceParameter(Accepts, variable)
                .ReplaceParameter(typeof (IBuildSession), session)
                .Body;
        }

        public Type Accepts { get { return typeof (T); } }
        public Type Returns { get { return typeof(T); } }
    }
}