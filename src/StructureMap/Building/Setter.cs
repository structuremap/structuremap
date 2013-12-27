using System;
using System.Linq.Expressions;
using System.Reflection;

namespace StructureMap.Building
{
    public class Setter
    {
        public static readonly ParameterExpression SessionParameter
            = Expression.Parameter(typeof(IBuildSession), "session");

        private readonly MemberInfo _member;

        public Setter(MemberInfo member, IDependencySource value)
        {
            _member = member;
            AssignedValue = value;
        }

        public IDependencySource AssignedValue { get; private set; }

        public string Description { get; private set; }

        public MemberBinding ToBinding(ParameterExpression session)
        {
            return Expression.Bind(_member, AssignedValue.ToExpression(session));
        }

        public LambdaExpression ToSetterLambda(Type concreteType, ParameterExpression target)
        {
            var lambdaType = typeof(Action<,>).MakeGenericType(typeof(IBuildSession), concreteType);
            var method = _member.As<PropertyInfo>().GetSetMethod();
            MethodCallExpression callSetMethod = Expression.Call(target, method, AssignedValue.ToExpression(SessionParameter));

            return Expression.Lambda(lambdaType, callSetMethod, SessionParameter, target);
        }
    }
}