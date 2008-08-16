namespace StructureMap.Pipeline
{
    public interface ILocationPolicy
    {
        object Build(BuildSession session, Instance instance);
    }

    //public class DefaultPolicy : ILocationPolicy
    //{
    //    public object Build(IInstanceCreator creator, Instance instance)
    //    {
    //        return instance.Build(creator);
    //    }
    //}
}