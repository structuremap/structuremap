using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.TypeRules;

namespace StructureMap.Building
{
    public class DefaultDependencySource : IDependencySource
    {
        private readonly Type _dependencyType;

        public static MethodInfo ContextMethod =
            typeof (IContext).GetMethods()
                .FirstOrDefault(x => x.Name == "GetInstance" && x.IsGenericMethod && x.GetParameters().Count() == 0);


        public DefaultDependencySource(Type dependencyType)
        {
            _dependencyType = dependencyType;
        }

        public Type DependencyType
        {
            get { return _dependencyType; }
        }

        public Type ReturnedType
        {
            get { return _dependencyType; }
        }

        public string Description
        {
            get { return "*Default of {0}*".ToFormat(_dependencyType.GetName()); }
        }

        public Expression ToExpression(ParameterExpression session, ParameterExpression context)
        {
            return Expression.Call(context, ContextMethod.MakeGenericMethod(_dependencyType));
        }
    }
}