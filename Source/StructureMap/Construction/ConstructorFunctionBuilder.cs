using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.Graph;

namespace StructureMap.Construction
{
    public class ConstructorFunctionBuilder<T>
    {
        public Func<IArguments, T> CreateBuilder()
        {
            Plugin plugin = PluginCache.GetPlugin(typeof (T));
            return CreateBuilder(plugin);
        }

        public Func<IArguments, T> CreateBuilder(Plugin plugin)
        {
            ConstructorInfo constructor = plugin.GetConstructor();

            var args = Expression.Parameter(typeof (IArguments), "x");

            
            var arguments = constructor.GetParameters().Select(param => ToParameterValueGetter(args, param.ParameterType, param.Name));

            var ctorCall = Expression.New(constructor, arguments);

            var lambda = Expression.Lambda(typeof (Func<IArguments, T>), ctorCall, args);
            return (Func<IArguments, T>) lambda.Compile();
        }

        public static Expression ToParameterValueGetter(ParameterExpression args, Type type, string argName)
        {
            MethodInfo method = typeof(IArguments).GetMethod("Get").MakeGenericMethod(type);
            return Expression.Call(args, method, Expression.Constant(argName));
        }
    }
}