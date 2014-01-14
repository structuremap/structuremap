using System;
using System.Linq;
using System.Linq.Expressions;

namespace StructureMap.Building.Interception
{
    public class FuncInterceptor<T> : IInterceptor
    {
        private readonly LambdaExpression _expression;

        public FuncInterceptor(Expression<Func<T, T>> expression)
        {
            _expression = expression;
        }

        public FuncInterceptor(Expression<Func<IBuildSession, T, T>> expression)
        {
            _expression = expression;
        }

        public string Description
        {
            get
            {
                return _expression
                    .ReplaceParameter(Accepts, Expression.Parameter(Accepts, Accepts.Name))
                    .ReplaceParameter(typeof(IBuildSession), Expression.Parameter(typeof(IBuildSession), "IBuildSession"))
                    .Body.ToString();

            }
        }
        public InterceptorRole Role { get { return InterceptorRole.Decorates; } }

        public Expression ToExpression(ParameterExpression session, ParameterExpression variable)
        {
            return _expression.ReplaceParameter(Accepts, variable)
                .ReplaceParameter(typeof (IBuildSession), session).Body;
        }

        public Type Accepts { get { return typeof (T); } }
        public Type Returns { get { return typeof (T); } }
    }
}