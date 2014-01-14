using System;
using System.Linq.Expressions;

namespace StructureMap.Building.Interception
{
    public interface IInterceptor : IDescribed
    {
        InterceptorRole Role { get; }

        Expression ToExpression(ParameterExpression session, ParameterExpression variable);

        Type Accepts { get; }
        Type Returns { get; }
    }
}