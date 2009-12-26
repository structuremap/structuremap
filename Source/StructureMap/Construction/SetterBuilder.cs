using System;
using System.Linq.Expressions;
using System.Reflection;

namespace StructureMap.Construction
{
    public class SetterBuilder<T>
    {
        public Action<IArguments, T> BuildMandatorySetter(string propertyName)
        {
            PropertyInfo property = typeof (T).GetProperty(propertyName);
            return BuildMandatorySetter(property);
        }

        public Action<IArguments, T> BuildMandatorySetter(PropertyInfo property)
        {
            ParameterExpression args = Expression.Parameter(typeof (IArguments), "args");
            ParameterExpression target = Expression.Parameter(typeof (T), "target");


            Expression getValue = ConstructorFunctionBuilder<T>.ToParameterValueGetter(args, property.PropertyType,
                                                                                       property.Name);
            MethodInfo method = property.GetSetMethod();

            MethodCallExpression callSetMethod = Expression.Call(target, method, getValue);

            LambdaExpression lambda = Expression.Lambda(typeof (Action<IArguments, T>), callSetMethod, args, target);

            return (Action<IArguments, T>) lambda.Compile();
        }

        public Action<IArguments, T> BuildOptionalSetter(PropertyInfo property)
        {
            string name = property.Name;
            Action<IArguments, T> func = BuildMandatorySetter(property);
            return (args, target) => { if (args.Has(name)) func(args, target); };
        }

        public Action<IArguments, T> BuildOptionalSetter(string propertyName)
        {
            PropertyInfo property = typeof (T).GetProperty(propertyName);
            return BuildOptionalSetter(property);
        }
    }
}