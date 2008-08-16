using System;

namespace StructureMap.Pipeline
{
    public interface IConfiguredInstance
    {
        string Name { get; }
        Type PluggedType { get; }
        Instance[] GetChildrenArray(string propertyName);
        string GetProperty(string propertyName);
        object GetChild(string propertyName, Type pluginType, BuildSession buildSession);
        object Build(Type pluginType, BuildSession session, InstanceBuilder builder);
        bool HasProperty(string propertyName);
    }
}