using System.Linq.Expressions;

namespace StructureMap.Building
{
    public static class Parameters
    {
        public static readonly ParameterExpression Session
            = Expression.Parameter(typeof (IBuildSession), "session");

        public static readonly ParameterExpression Context
            = Expression.Variable(typeof (IContext), "context");

        public static Expression SessionToContext()
        {
            return Expression.Assign(Context, Session);
        }
    }
}