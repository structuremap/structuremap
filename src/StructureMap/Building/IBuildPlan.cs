using System;

namespace StructureMap.Building
{
    public interface IBuildPlan
    {
        Delegate ToDelegate();
    }
}