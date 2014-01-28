using System;

namespace StructureMap.Building
{
    public interface IBuildPlan : IDescribed
    {
        object Build(IBuildSession session, IContext context);

        Type ReturnedType { get; }
    }
}