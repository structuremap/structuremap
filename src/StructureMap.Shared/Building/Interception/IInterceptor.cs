using System;
using System.Linq.Expressions;

namespace StructureMap.Building.Interception
{
    public interface IInterceptor : IDescribed
    {
        // "activates" or "decorates"
        InterceptorRole Role { get; }

        Expression ToExpression(Policies policies, ParameterExpression session, ParameterExpression variable);

        Type Accepts { get; }
        Type Returns { get; }
    }
}