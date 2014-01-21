using System;
using System.Linq.Expressions;

namespace StructureMap.Building
{
    public interface IBuildPlan : IDescribed
    {
        object Build(IBuildSession session, IContext context);

        Expression ToExpression(ParameterExpression session, ParameterExpression context);

        Type ReturnedType { get; }
    }
}