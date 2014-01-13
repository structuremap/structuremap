using System;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.Util;

namespace StructureMap.Building
{
    public static class TryCatchWrapper
    {
        public static ParameterExpression EX = Expression.Parameter(typeof (Exception), "ex");
        public static MethodInfo DescriptionMethod = typeof (IDescribed).GetProperty("Description").GetGetMethod();

        private static readonly Cache<Type, ConstructorInfo> _constructors =
            new Cache<Type, ConstructorInfo>(
                type => type.GetConstructor(new Type[] {typeof (string), typeof (Exception)}));


        public static Expression Wrap<T>(Type returnType, Expression expression,
            Expression<Func<string>> descriptionSource) where T : StructureMapException
        {
            var description = descriptionSource.Body;

            return Wrap<T>(returnType, expression, description);
        }

        public static Expression Wrap<T>(Type returnType, Expression expression, IDescribed described)
            where T : StructureMapException
        {
            var description = Expression.Call(Expression.Constant(described), DescriptionMethod);
            return Wrap<T>(returnType, expression, description);
        }

        public static Expression Wrap<T>(Type returnType, Expression expression, string descriptionString)
            where T : StructureMapException
        {
            var description = Expression.Constant(descriptionString);
            return Wrap<T>(returnType, expression, description);
        }

        public static Expression Wrap<T>(Type returnType, Expression expression, Expression description)
            where T : StructureMapException
        {
            var constructor = _constructors[typeof (T)];

            var newSmEx = Expression.New(constructor, description, EX);

            var genericThrow = Expression.Throw(newSmEx);

            var genericCatch = Expression.Catch(EX, Expression.Block(genericThrow, Expression.Default(returnType)));

            var smParameter = Expression.Parameter(typeof (StructureMapException), "ex");

            var smPush = Expression.Call(smParameter, StructureMapException.PushMethod, description);
            var rethrow = Expression.Throw(smParameter);

            var pushAndReThrowBlock = Expression.Block(returnType, smPush, rethrow, Expression.Default(returnType));
            var smCatch = Expression.Catch(smParameter, pushAndReThrowBlock);


            return Expression.TryCatch(expression, smCatch, genericCatch);
        }
    }
}