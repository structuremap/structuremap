using System.Linq.Expressions;

namespace StructureMap.Building
{
    public static class Parameters
    {
        public static readonly ParameterExpression Session
            = Expression.Parameter(typeof(IBuildSession), "session");
    }
}