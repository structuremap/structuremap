namespace StructureMap.DataAccess
{
    public interface IInitializable
    {
        void Initialize(IDatabaseEngine engine);
    }
}