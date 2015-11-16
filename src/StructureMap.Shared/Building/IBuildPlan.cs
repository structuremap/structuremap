using System;

namespace StructureMap.Building
{
    public interface IBuildPlan : IDescribed, IBuildPlanVisitable
    {
        object Build(IBuildSession session, IContext context);

        Type ReturnedType { get; }
    }
}