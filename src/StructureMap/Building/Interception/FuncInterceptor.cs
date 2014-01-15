using System;
using System.Linq;
using System.Linq.Expressions;

namespace StructureMap.Building.Interception
{
    public class FuncInterceptor<T> : IInterceptor
    {
        private readonly LambdaExpression _expression;
        private readonly string _description;

        public FuncInterceptor(Expression<Func<T, T>> expression, string description = null)
        {
            _expression = expression;
            _description = description;
        }

        public FuncInterceptor(Expression<Func<IBuildSession, T, T>> expression, string description = null)
        {
            _expression = expression;
            _description = description;
        }

        public string Description
        {
            get
            {
                return _description ?? _expression
                    .ReplaceParameter(Accepts, Expression.Parameter(Accepts, Accepts.Name))
                    .ReplaceParameter(typeof(IBuildSession), Expression.Parameter(typeof(IBuildSession), "IBuildSession"))
                    .Body.ToString();

            }
        }
        public InterceptorRole Role { get { return InterceptorRole.Decorates; } }

        public Expression ToExpression(ParameterExpression session, ParameterExpression variable)
        {
            var body = _expression.ReplaceParameter(Accepts, variable)
                .ReplaceParameter(typeof (IBuildSession), session).Body;

            return Expression.Convert(body, typeof (T));
        }

        public Type Accepts { get { return typeof (T); } }
        public Type Returns { get { return typeof (T); } }
    }
}