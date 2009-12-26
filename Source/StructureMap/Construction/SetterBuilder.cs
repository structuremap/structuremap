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
            var args = Expression.Parameter(typeof (IArguments), "args");
            var target = Expression.Parameter(typeof (T), "target");


            var getValue = ConstructorFunctionBuilder<T>.ToParameterValueGetter(args, property.PropertyType, property.Name);
            var method = property.GetSetMethod();

            var callSetMethod = Expression.Call(target, method, getValue);

            var lambda = Expression.Lambda(typeof (Action<IArguments, T>), callSetMethod, args, target);

            return (Action<IArguments, T>) lambda.Compile();
        }

        public Action<IArguments, T> BuildOptionalSetter(PropertyInfo property)
        {
            var name = property.Name;
            var func = BuildMandatorySetter(property);
            return (args, target) =>
            {
                if (args.Has(name)) func(args, target);
            };
        }

        public Action<IArguments, T> BuildOptionalSetter(string propertyName)
        {
            PropertyInfo property = typeof(T).GetProperty(propertyName);
            return BuildOptionalSetter(property);
        }
    }
}