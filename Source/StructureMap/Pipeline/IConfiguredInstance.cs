namespace StructureMap.Pipeline
{
    public interface IConfiguredInstance
    {
        Instance[] GetChildrenArray(string propertyName);
        string GetProperty(string propertyName);
        object GetChild(string propertyName, string typeName, IInstanceCreator instanceCreator);
        InstanceBuilder FindBuilder(InstanceBuilderList builders);

        string ConcreteKey
        {
            get;
            set;
        }

        string Name { get;}
    }
}