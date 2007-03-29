namespace StructureMap.Verification
{
    public interface IStartUp
    {
        IStartUp WriteProblemsTo(string fileName);
        IStartUp FailOnException();
        IStartUp WriteAllTo(string fileName);
    }
}