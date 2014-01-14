using System;
using System.Linq;
using System.Linq.Expressions;

namespace StructureMap.Building.Interception
{
    public class ActivatorInterceptor<T> : IInterceptor
    {
        private readonly LambdaExpression _action;

        public ActivatorInterceptor(Expression<Action<T>> action)
        {
            _action = action;
        }

        public ActivatorInterceptor(Expression<Action<IBuildSession, T>> action)
        {
            _action = action;
        } 

        public string Description
        {
            get
            {
                return _action
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