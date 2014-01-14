using System;
using System.Linq;
using System.Linq.Expressions;

namespace StructureMap.Building.Interception
{
    public class ActivatorInterceptor<T> : IInterceptor
    {
        private readonly Expression<Action<T>> _action;

        public ActivatorInterceptor(Expression<Action<T>> action)
        {
            _action = action;
        }

        public string Description
        {
            get
            {
                var description = _action.Body.ToString();
                var parameterName = _action.Parameters.Single().Name;

                return description.Replace(parameterName + ".", typeof (T).Name + ".");
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
            return ParameterRewriter.ReplaceParameter(Accepts, _action, variable).Body;
        }

        public Type Accepts { get { return typeof (T); } }
        public Type Returns { get { return typeof(T); } }
    }
}