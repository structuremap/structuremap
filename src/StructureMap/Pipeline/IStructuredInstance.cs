namespace StructureMap.Pipeline
{
    public interface IStructuredInstance
    {
        Instance GetChild(string name);
        Instance[] GetChildArray(string name);
        void RemoveKey(string name);
    }
}