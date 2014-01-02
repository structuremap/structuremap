namespace StructureMap.Building
{
    public interface IBuildPlan
    {
        object Build(IBuildSession session);
    }
}