using System;
using System.Linq.Expressions;

namespace StructureMap.Building
{
    public interface IBuildPlan : IDescribed
    {
        [Obsolete("Think this goes away")]
        object Build(IBuildSession session);
        Expression ToExpression(ParameterExpression session);
    }
}