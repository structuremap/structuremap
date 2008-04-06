namespace StructureMap
{
    public interface IInstanceCreator
    {
        InstanceMemento DefaultMemento { get; }
        object BuildInstance(InstanceMemento memento);
    }
}