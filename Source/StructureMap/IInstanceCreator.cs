namespace StructureMap
{
    public interface IInstanceCreator
    {
        object BuildInstance(InstanceMemento memento);
        InstanceMemento DefaultMemento{ get;}
    }
}