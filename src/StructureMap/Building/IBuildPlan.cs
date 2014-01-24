using System;
using System.Linq.Expressions;

namespace StructureMap.Building
{
    public interface IBuildPlan : IDescribed
    {
        object Build(IBuildSession session, IContext context);

        Type ReturnedType { get; }
    }
}