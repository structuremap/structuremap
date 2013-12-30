using System;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.Pipeline;

namespace StructureMap.Building
{
    public class LifecycleDependencySource : IDependencySource
    {
        public static MethodInfo SessionMethod =
            ReflectionHelper.GetMethod<IBuildSession>(x => x.ResolveFromLifecycle(null, null));

        private readonly Type _pluginType;
        private readonly Instance _instance;

        public LifecycleDependencySource(Type pluginType, Instance instance)
        {
            _pluginType = pluginType;
            _instance = instance;
        }

        public Type PluginType
        {
            get { return _pluginType; }
        }

        public Instance Instance
        {
            get { return _instance; }
        }

        public string Description { get; private set; }

        public Expression ToExpression(ParameterExpression session)
        {
            var typeArg = Expression.Constant(_pluginType);
            var instanceArg = Expression.Constant(_instance);

            var method = Expression.Call(session, SessionMethod, typeArg, instanceArg);
            

            return Expression.Convert(method, _pluginType);
        }
    }
}