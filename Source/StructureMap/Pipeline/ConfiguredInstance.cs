using System;

namespace StructureMap.Pipeline
{
    public interface IConfiguredInstance
    {
        InstanceMemento[] GetChildrenArray(string propertyName);
        string GetProperty(string propertyName);
        object GetChild(string propertyName, string typeName, IInstanceCreator instanceCreator);
    }

    public class ConfiguredInstance : Instance
    {
        protected override object build(Type type, IInstanceCreator creator)
        {
            throw new NotImplementedException();
        }
    }
}