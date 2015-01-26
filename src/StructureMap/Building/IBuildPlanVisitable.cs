using StructureMap.Diagnostics;

namespace StructureMap.Building
{
    public interface IBuildPlanVisitable
    {
        void AcceptVisitor(IBuildPlanVisitor visitor);
    }
}