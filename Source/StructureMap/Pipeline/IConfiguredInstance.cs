using System;

namespace StructureMap.Pipeline
{
    public interface IConfiguredInstance
    {
        string Name { get; }
        Type PluggedType { get; }



        [Obsolete]
        Instance[] GetChildrenArray(string propertyName);

        [Obsolete]
        string GetProperty(string propertyName);
        
        
        object Get(string propertyName, Type pluginType, BuildSession buildSession);

        object Build(Type pluginType, BuildSession session, InstanceBuilder builder);

        bool HasProperty(string propertyName);

        [Obsolete]
        void SetProperty(string name, string value);
        
        
        void Set(string name, Instance instance);

        [Obsolete]
        void SetChildArray(string name, Type type, Instance[] children);
    }
}