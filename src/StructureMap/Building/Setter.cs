using System;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.TypeRules;

namespace StructureMap.Building
{
    public class Setter : IDescribed
    {
        private readonly Type _setterType;
        private readonly MemberInfo _member;

        public Setter(Type setterType, MemberInfo member, IDependencySource value)
        {
            _setterType = setterType;
            _member = member;
            AssignedValue = value;
        }

        public IDependencySource AssignedValue { get; private set; }

        public Type SetterType
        {
            get { return _setterType; }
        }

        public string Description
        {
            get
            {

                return "Set {1} {0} = {2}".ToFormat(_member.Name, _setterType.GetTypeName(),
                    AssignedValue.Description);
            }
        }

        public string Title
        {
            get
            {
                return "Set {1} {0} = "
                    .ToFormat(_member.Name, _setterType.GetTypeName());
            }
        }

        public MemberBinding ToBinding(ParameterExpression session, ParameterExpression context)
        {
            return Expression.Bind(_member, AssignedValue.ToExpression(session, context));
        }

        public LambdaExpression ToSetterLambda(Type concreteType, ParameterExpression target)
        {
            var lambdaType = typeof (Action<,,>).MakeGenericType(typeof (IBuildSession), typeof (IContext), concreteType);
            var method = _member.As<PropertyInfo>().GetSetMethod();
            var callSetMethod = Expression.Call(target, method,
                AssignedValue.ToExpression(Parameters.Session, Parameters.Context));

            return Expression.Lambda(lambdaType, callSetMethod, Parameters.Session, Parameters.Context, target);
        }
    }
}