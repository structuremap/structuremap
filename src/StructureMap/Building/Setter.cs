using System;
using System.Linq.Expressions;
using System.Reflection;

namespace StructureMap.Building
{
    public class Setter : IDescribed
    {
        private readonly MemberInfo _member;

        public Setter(MemberInfo member, IDependencySource value)
        {
            _member = member;
            AssignedValue = value;
        }

        public IDependencySource AssignedValue { get; private set; }

        public string Description
        {
            get
            {
                return _member.Name + " = " + AssignedValue.Description;
            }
        }

        public MemberBinding ToBinding(ParameterExpression session, ParameterExpression context)
        {
            return Expression.Bind(_member, AssignedValue.ToExpression(session, context));
        }

        public LambdaExpression ToSetterLambda(Type concreteType, ParameterExpression target)
        {
            var lambdaType = typeof (Action<,,>).MakeGenericType(typeof (IBuildSession), typeof(IContext), concreteType);
            var method = _member.As<PropertyInfo>().GetSetMethod();
            var callSetMethod = Expression.Call(target, method, AssignedValue.ToExpression(Parameters.Session, Parameters.Context));

            return Expression.Lambda(lambdaType, callSetMethod, Parameters.Session, Parameters.Context, target);
        }
    }
}