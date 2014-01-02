using System;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.Pipeline;

namespace StructureMap.Building
{
    public class InstanceDependencySource : IDependencySource
    {
        public static readonly MethodInfo BuildMethod = ReflectionHelper.GetMethod<Instance>(x => x.Build(null, null));

        private readonly Type _pluginType;
        private readonly Instance _instance;

        public InstanceDependencySource(Type pluginType, Instance instance)
        {
            _pluginType = pluginType;
            _instance = instance;
        }

        public string Description { get; private set; }
        public Expression ToExpression(ParameterExpression session)
        {
            return Expression.Call(Expression.Constant(_instance), BuildMethod, Expression.Constant(_pluginType), session);
        }
    }
}