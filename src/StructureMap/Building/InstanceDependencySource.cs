using System;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.Pipeline;

namespace StructureMap.Building
{
    // No unit tests for this, but it's tested heavily within integration tests
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
            var instanceConstant = Expression.Constant(_instance, typeof (Instance));
            var typeConstant = Expression.Constant(_pluginType, typeof (Type));
            var call = Expression.Call(instanceConstant, BuildMethod, typeConstant, session);


            return Expression.Convert(call, _pluginType);
        }

        public Type ReturnedType
        {
            get
            {
                return _instance.ConcreteType;
            }
        }
    }
}