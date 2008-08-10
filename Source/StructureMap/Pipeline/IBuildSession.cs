using System;

namespace StructureMap.Pipeline
{
    public interface IBuildSession
    {
        object CreateInstance(Type pluginType, string name);
        object CreateInstance(Type pluginType, Instance instance);
        Array CreateInstanceArray(Type pluginType, Instance[] instances);
        object CreateInstance(Type pluginType);
        object ApplyInterception(Type pluginType, object actualValue);
        void RegisterDefault(Type pluginType, object defaultObject);
    }
}