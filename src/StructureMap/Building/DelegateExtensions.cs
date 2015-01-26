using System;
using System.Linq.Expressions;
using StructureMap.Building.Interception;

namespace StructureMap.Building
{
    public static class DelegateExtensions
    {
        public static Func<IBuildSession, IContext, T> BuilderOf<T>(this Delegate @delegate)
        {
            return @delegate.As<Func<IBuildSession, IContext, T>>();
        }

        public static T Build<T>(this IBuildPlan plan, IBuildSession session) where T : class
        {
            return plan.Build(session, session.As<IContext>()).As<T>();
        }

        public static LambdaExpression ReplaceParameter(this LambdaExpression expression, Type acceptsType,
            ParameterExpression newParam)
        {
            return ParameterRewriter.ReplaceParameter(acceptsType, expression, newParam);
        }
    }
}