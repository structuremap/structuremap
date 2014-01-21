using System;
using System.Linq.Expressions;

namespace StructureMap.Building
{
    public interface IDependencySource : IDescribed
    {
        Expression ToExpression(ParameterExpression session, ParameterExpression context);
        Type ReturnedType { get; }
    }
}