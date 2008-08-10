using System;

namespace StructureMap.Pipeline
{
    public interface IConfiguredInstance
    {
        string Name { get; }
        Type PluggedType { get; }
        Instance[] GetChildrenArray(string propertyName);
        string GetProperty(string propertyName);
        object GetChild(string propertyName, Type pluginType, IBuildSession buildSession);
        object Build(Type pluginType, IBuildSession session, InstanceBuilder builder);
        bool HasProperty(string propertyName);
    }
}