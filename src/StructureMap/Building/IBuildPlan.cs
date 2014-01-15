using System;
using System.Collections;
using System.Linq.Expressions;
using StructureMap.Pipeline;

namespace StructureMap.Building
{
    public interface IBuildPlan : IDescribed
    {
        object Build(IBuildSession session);

        Expression ToExpression(ParameterExpression session);

        Type ReturnedType { get; }
    }
}