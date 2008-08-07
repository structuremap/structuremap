using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace StructureMap
{
    internal static class ReflectionHelper
    {
        public static PropertyInfo GetProperty<MODEL>(Expression<Func<MODEL, object>> expression)
        {
            MemberExpression memberExpression = getMemberExpression(expression);
            return (PropertyInfo)memberExpression.Member;
        }

        public static PropertyInfo GetProperty<MODEL, T>(Expression<Func<MODEL, T>> expression)
        {
            MemberExpression memberExpression = getMemberExpression(expression);
            return (PropertyInfo)memberExpression.Member;
        }

        private static MemberExpression getMemberExpression<MODEL, T>(Expression<Func<MODEL, T>> expression)
        {
            MemberExpression memberExpression = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression)expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }



            if (memberExpression == null) throw new ArgumentException("Not a member access", "member");
            return memberExpression;
        }

        public static MethodInfo GetMethod<T>(Expression<Func<T, object>> expression)
        {

            MethodCallExpression methodCall =
                expression.Body is UnaryExpression
                    ? (MethodCallExpression) ((UnaryExpression) expression.Body).Operand
                    : (MethodCallExpression) expression.Body;

            return methodCall.Method;
        }

        public static MethodInfo GetMethod<T, U>(Expression<Func<T, U>> expression)
        {
            MethodCallExpression methodCall = (MethodCallExpression)expression.Body;
            return methodCall.Method;
        }

        public static MethodInfo GetMethod<T, U, V>(Expression<Func<T, U, V>> expression)
        {
            MethodCallExpression methodCall = (MethodCallExpression)expression.Body;
            return methodCall.Method;
        }

    }
}
