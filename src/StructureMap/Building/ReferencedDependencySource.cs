using System;
using System.Linq.Expressions;
using System.Reflection;

namespace StructureMap.Building
{
    public class ReferencedDependencySource : IDependencySource
    {
        private readonly Type _dependencyType;
        private readonly string _name;

        public static MethodInfo SessionMethod =
            typeof(IBuildSession).GetMethod("GetInstance", new Type[]{typeof(string)});

        public ReferencedDependencySource(Type dependencyType, string name)
        {
            _dependencyType = dependencyType;
            _name = name;
        }

        public string Description { get; private set; }
        public Expression ToExpression(ParameterExpression session)
        {
            return Expression.Call(session, SessionMethod.MakeGenericMethod(_dependencyType), Expression.Constant(_name));
        }
    }
}