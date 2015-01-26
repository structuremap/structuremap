using System;
using System.Linq.Expressions;
using StructureMap.Diagnostics;

namespace StructureMap.Building
{
    public interface IDependencySource : IDescribed
    {
        Expression ToExpression(ParameterExpression session, ParameterExpression context);
        Type ReturnedType { get; }

        void AcceptVisitor(IDependencyVisitor visitor);
    }
}