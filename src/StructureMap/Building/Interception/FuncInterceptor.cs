using System;
using System.Linq.Expressions;
using StructureMap.TypeRules;

namespace StructureMap.Building.Interception
{
    public class FuncInterceptor<T> : FuncInterceptor<T, T>
    {
        public FuncInterceptor(Expression<Func<T, T>> expression, string description = null) : base(expression, description)
        {
        }

        public FuncInterceptor(Expression<Func<IContext, T, T>> expression, string description = null) : base(expression, description)
        {
        }
    }

    public class FuncInterceptor<T, TPluginType> : IInterceptor where T : TPluginType
    {
        private readonly LambdaExpression _expression;
        private readonly string _description;

        public FuncInterceptor(Expression<Func<T, TPluginType>> expression, string description = null)
        {
            _expression = expression;
            _description = description;
        }

        public FuncInterceptor(Expression<Func<IContext, T, TPluginType>> expression, string description = null)
        {
            _expression = expression;
            _description = description;
        }

        public string Description
        {
            get
            {
                var bodyDescription = _description ?? _expression
                    .ReplaceParameter(Accepts, Expression.Parameter(Accepts, Accepts.Name))
                    .ReplaceParameter(typeof (IContext), Expression.Parameter(typeof (IContext), "IContext"))
                    .Body.ToString();

                return "FuncInterceptor of {0}: {1}".ToFormat(typeof (T).GetFullName(), bodyDescription);
            }
        }

        public InterceptorRole Role
        {
            get { return InterceptorRole.Decorates; }
        }

        public Expression ToExpression(Policies policies, ParameterExpression context, ParameterExpression variable)
        {
            var body = _expression.ReplaceParameter(Accepts, variable)
                .ReplaceParameter(typeof (IContext), context).Body;

            return Expression.Convert(body, typeof (T));
        }

        public IInterceptorPolicy ToPolicy()
        {
            return new InterceptorPolicy<T>(this);
        }

        public Type Accepts
        {
            get { return typeof (T); }
        }

        public Type Returns
        {
            get { return typeof (TPluginType); }
        }

        protected bool Equals(FuncInterceptor<T, TPluginType> other)
        {
            return Equals(_expression, other._expression);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FuncInterceptor<T, TPluginType>) obj);
        }

        public override int GetHashCode()
        {
            return (_expression != null ? _expression.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return "Interceptor of {0}: {1}".ToFormat(typeof (T).GetFullName(), Description);
        }
    }
}